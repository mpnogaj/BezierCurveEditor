using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using BezierCurveEditor.Controls;

namespace BezierCurveEditor
{
	public partial class MainWindow : Form
	{
		private readonly string _defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		private readonly string _fileDialogsFilter = @"Drawings (*.drw)|*.drw";


		private string _fileName = string.Empty;
		private string FileName
		{
			get => _fileName;
			set
			{
				if (value == FileName) return;
				_fileName = value;
			}
		}

		public MainWindow()
		{
			InitializeComponent();
			canvas.Resizeable();

			canvas.StatusChanged += (sender, args) => { canvasModeStatusLabel.Text = canvas.Status; };

			canvas.Curves.CollectionChanged += CurvesOnCollectionChanged;
			canvas.PointsHierarchyChanged += CanvasOnPointsHierarchyChanged;

			canvas.ModeChanged += CanvasOnModeChanged;
		}

		private void CanvasOnModeChanged(object sender, ModeChangedEventArgs e)
		{
			var selectedNode = curvesView.SelectedNode;
			if ((e.Mode == ModeType.Normal || e.Mode == ModeType.Insert) && selectedNode != null)
			{
				switch (selectedNode)
				{
					case ExtTreeNode<BezierCurve> selectedCurve:
						selectedCurve.Data.Selected = false;
						break;
					case ExtTreeNode<DraggablePoint> selectedPoint:
						selectedPoint.Data.Curve.Selected = false;
						break;
				}

				curvesView.SelectedNode = null;
			}
		}

		private void CanvasOnPointsHierarchyChanged(object sender, PointsHierarchyChangedEventArgs e)
		{
			var treeNodes = curvesView.Nodes;
			foreach (TreeNode treeNode in treeNodes)
			{
				if (treeNode is ExtTreeNode<BezierCurve> curveNode)
				{
					if (curveNode.Data == e.Curve)
					{
						treeNode.Nodes.Clear();
						var children = curveNode.Data.DraggablePoints
						                        .Select((t, j) => new ExtTreeNode<DraggablePoint>($"Point {j + 1}", t))
						                        .Cast<TreeNode>().ToArray();

						treeNode.Nodes.AddRange(children);
					}
				}
			}
		}

		private void CurvesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				var toRemove = new List<TreeNode>();
				foreach (TreeNode node in curvesView.Nodes)
				{
					if (node is ExtTreeNode<BezierCurve> curveNode && e.OldItems.Contains(curveNode))
					{
						toRemove.Add(node);
					}
				}

				foreach (var node in toRemove)
				{
					curvesView.Nodes.Remove(node);
				}
			}

			if (e.NewItems != null)
			{

				var curves = e.NewItems.Cast<BezierCurve>().ToList();
				var treeNodes = new List<TreeNode>(curves.Count);

				foreach (var curve in curves)
				{
					var points = curve.DraggablePoints;
					var children = points.Select((t, j) => new ExtTreeNode<DraggablePoint>($"Point {j + 1}", t))
					                     .Cast<TreeNode>().ToArray();

					treeNodes.Add(new ExtTreeNode<BezierCurve>("Curve {0}", curve, children));
				}

				curvesView.Nodes.AddRange(treeNodes.ToArray());

				for (var i = 0; i < curvesView.Nodes.Count; i++)
				{
					var node = curvesView.Nodes[i];
					node.Text = string.Format(node.Text, i + 1);
				}
			}
		}

		private void MainWindow_KeyUp(object sender, KeyEventArgs e)
		{
			canvas.HandleKeyPressed(e.KeyCode);
		}

		private void curvesView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
		{
			var oldSelectedNode = curvesView.SelectedNode;

			switch (oldSelectedNode)
			{
				case ExtTreeNode<BezierCurve> oldSelectedCurve:
					oldSelectedCurve.Data.Selected = false;
					break;
				case ExtTreeNode<DraggablePoint> oldSelectedPoint:
					oldSelectedPoint.Data.PointSelected = false;
					break;
			}

			var newSelectedNode = e.Node;

			switch (newSelectedNode)
			{
				case ExtTreeNode<BezierCurve> newSelectedCurve:
					newSelectedCurve.Data.Selected = true;
					break;
				case ExtTreeNode<DraggablePoint> newSelectedPoint:
					newSelectedPoint.Data.PointSelected = true;
					break;
			}

			canvas.ChangeMode(ModeType.Move);
		}

		private void curvesView_KeyUp(object sender, KeyEventArgs e)
		{
			var selectedNode = curvesView.SelectedNode;
			if (selectedNode != null)
			{
				if (e.KeyCode == Keys.Delete)
				{
					switch (selectedNode)
					{
						case ExtTreeNode<BezierCurve> newSelectedCurve:
							newSelectedCurve.Data.DeleteCurve();
							break;
						case ExtTreeNode<DraggablePoint> newSelectedPoint:
							newSelectedPoint.Data.Curve.DeletePoint(newSelectedPoint.Data);
							break;
					}
				}
			}
		}


		private bool SaveAs()
		{
			var sfd = new SaveFileDialog()
			{
				InitialDirectory = _defaultPath,
				Filter = _fileDialogsFilter,
				RestoreDirectory = true,
				AddExtension = true,
				DefaultExt = "drw",
				Title = "Save as"
			};

			if (sfd.ShowDialog() != DialogResult.OK) return false;

			FileName = sfd.FileName;

			return true;
		}

		private void Save()
		{
			var dataModel = canvas.SaveCurves();
			var json = JsonSerializer.Serialize(dataModel);
			using var sw = new StreamWriter(FileName, append: false);
			sw.Write(json);
		}

		private void newFileMenu_Click(object sender, EventArgs e)
		{
			FileName = string.Empty;
			canvas.Clear();
			curvesView.Nodes.Clear();
		}

		private void openFileMenu_Click(object sender, System.EventArgs e)
		{
			var ofd = new OpenFileDialog
			{
				InitialDirectory = _defaultPath,
				Filter = _fileDialogsFilter,
				RestoreDirectory = true,
				Multiselect = false,
				Title = "Open"
			};

			if (ofd.ShowDialog() != DialogResult.OK) return;
			
			var fileName = ofd.FileName;
			using var fileStream = ofd.OpenFile();
			var dataModel = JsonSerializer.Deserialize<DataModel>(fileStream);
			FileName = fileName;

			canvas.Clear();
			curvesView.Nodes.Clear();
			canvas.LoadCurves(dataModel);
		}

		private void saveFileMenu_Click(object sender, EventArgs e)
		{
			if (FileName == string.Empty)
			{
				if (!SaveAs()) return;
			}

			Save();
		}

		private void saveAsFileMenu_Click(object sender, EventArgs e)
		{
			if (!SaveAs()) return;

			Save();
		}

		private void exitFileMenu_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}