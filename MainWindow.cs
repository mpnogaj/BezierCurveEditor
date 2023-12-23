using System;
using System.CodeDom;
using System.Diagnostics;
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

			canvas.StatusChanged += Canvas_StatusChanged;

			canvas.CurveAdded += Canvas_CurveAdded;
			canvas.CurveRemoved += Canvas_CurveRemoved;

			canvas.PointAddedRemoved += Canvas_PointAddedRemoved; ;

			canvas.SelectedItemChanged += Canvas_SelectedItemChanged;

			canvas.ErrorChanged += (sender, args) =>
			{
				canvasErrorLabel.Text = canvas.Error;
			};

			canvasModeStatusLabel.Text = canvas.CurrentMode.StatusBarText;
		}

		private void Canvas_PointAddedRemoved(object sender, PointAddedRemovedEventArgs e)
		{
			var curveNode = curvesView.Nodes
				.OfType<ExtTreeNode<BezierCurve>>()
				.FirstOrDefault(x => x.Data == e.Point.Curve);

			if (curveNode == null)
				throw new InvalidOperationException("Point's curve is not on the list!");

			switch (e.Method)
			{
				case Method.Added:
					curveNode.Nodes.Insert(e.Index, new ExtTreeNode<DraggablePoint>("Point", e.Point));
					break;
				case Method.Removed:
					curveNode.Nodes.RemoveAt(e.Index);
					break;
				default:
					Debug.WriteLine("Unknown method");
					break;
			}
			
			FixNames("Point", curveNode.Nodes);
		}

		private void Canvas_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
		{
			switch (e.Object)
			{
				case null:
					curvesView.SelectedNode = null;
					break;
				case BezierCurve curve:
				{
					var nodeToSelect = curvesView.Nodes
						.OfType<ExtTreeNode<BezierCurve>>()
						.FirstOrDefault(x => x.Data == curve);

					curvesView.SelectedNode = nodeToSelect;
					break;
				}
				case DraggablePoint point:
				{
					var curveNode = curvesView.Nodes
						.OfType<ExtTreeNode<BezierCurve>>()
						.FirstOrDefault(x => x.Data == point.Curve);

					if (curveNode == null)
					{
						curvesView.SelectedNode = null;
						return;
					}

					var pointNode = curveNode.Nodes
						.OfType<ExtTreeNode<DraggablePoint>>()
						.FirstOrDefault(x => x.Data == point);

					curvesView.SelectedNode = pointNode;
					break;
				}
			}
		}

		private void Canvas_CurveAdded(object sender, CurvesUpdatedEventArgs e)
		{
			var pointsNodes =
				e.Curve.DraggablePoints.Select((x, i) => new ExtTreeNode<DraggablePoint>($"Point {i + 1}", x))
					.Cast<TreeNode>()
					.ToArray();
			var curveNode = new ExtTreeNode<BezierCurve>($"Curve {curvesView.Nodes.Count + 1}", e.Curve, pointsNodes);

			curvesView.Nodes.Insert(e.Index, curveNode);
		}

		private void Canvas_CurveRemoved(object sender, CurvesUpdatedEventArgs e)
		{
			var node = curvesView.Nodes[e.Index];
			if (curvesView.SelectedNode == node)
			{
				curvesView.SelectedNode = null;
			}
			curvesView.Nodes.RemoveAt(e.Index);
			FixNames("Curve", curvesView.Nodes);
		}

		private void Canvas_StatusChanged(object sender, EventArgs e)
		{
			canvasModeStatusLabel.Text = canvas.Status;
		}

		private void MainWindow_KeyUp(object sender, KeyEventArgs e)
		{
			canvas.HandleKeyPressed(e.KeyCode);
			//e.Handled = true;
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

			e.Handled = true;
			e.SuppressKeyPress = true;
		}


		private bool PickFileName()
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
			if (canvas.UnsavedChanges)
			{
				var handled = UnsavedChangesPopup();
				if (!handled) return;
			}

			FileName = string.Empty;
			canvas.Clear(false);
			curvesView.Nodes.Clear();
		}

		private void openFileMenu_Click(object sender, System.EventArgs e)
		{
			if (canvas.UnsavedChanges)
			{
				var handled = UnsavedChangesPopup();
				if (!handled) return;
			}

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
				if (!PickFileName()) return;
			}

			Save();
		}

		private void saveAsFileMenu_Click(object sender, EventArgs e)
		{
			if (!PickFileName()) return;

			Save();
		}

		private void exitFileMenu_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private bool UnsavedChangesPopup()
		{
			var dialogRes = MessageBox.Show("You have unsaved changes. Do you want to save them?", "Unsaved changes",
				MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

			switch (dialogRes)
			{
				case DialogResult.Yes:
					if (FileName == string.Empty)
					{
						if (!PickFileName()) return false;
					}

					Save();
					return true;
				case DialogResult.No:
					return true;
				case DialogResult.Cancel:
					return false;
			}

			return false;
		}

		private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (canvas.UnsavedChanges)
			{
				var handled = UnsavedChangesPopup();
				e.Cancel = !handled;
			}
		}

		private void curvesView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			canvas.SelectedItem = e.Node switch
			{
				ExtTreeNode<BezierCurve> curveNode => curveNode.Data,
				ExtTreeNode<DraggablePoint> pointNode => pointNode.Data,
				_ => null
			};
		}

		private void FixNames(string prefix, TreeNodeCollection nodes)
		{
			for (var i = 0; i < nodes.Count; i++)
			{
				nodes[i].Text = $"{prefix} {i + 1}";
			}
		}

		private void showHelpMenu_Click(object sender, EventArgs e)
		{

		}
	}
}