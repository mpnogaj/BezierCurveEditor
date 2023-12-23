﻿using System;
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
			
			canvas.PointsHierarchyChanged += CanvasOnPointsHierarchyChanged;

			canvas.ModeChanged += CanvasOnModeChanged;
		}

		private void Canvas_CurveAdded(object sender, CurvesUpdatedEventArgs e)
		{
			var pointsNodes =
				e.Curve.DraggablePoints.Select((x, i) => new ExtTreeNode<DraggablePoint>($"Point {i}", x))
					.Cast<TreeNode>()
					.ToArray();
			var curveNode = new ExtTreeNode<BezierCurve>("Curve", e.Curve, pointsNodes);

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
		}

		private void Canvas_StatusChanged(object sender, EventArgs e)
		{
			canvasModeStatusLabel.Text = canvas.Status;
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

		private void MainWindow_KeyUp(object sender, KeyEventArgs e)
		{
			e.SuppressKeyPress = canvas.HandleKeyPressed(e.KeyCode);
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
	}
}