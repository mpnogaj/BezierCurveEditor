using System;
using System.Windows.Forms;

namespace BezierCurveEditor
{
	public class ExtTreeNode<T> : TreeNode
	{
		public T Data { get; }

		public ExtTreeNode(string text, T data, TreeNode[] children) : base(text, children)
		{
			Data = data;
		}

		public ExtTreeNode(string text, T data) : this(text, data, Array.Empty<TreeNode>())
		{
		}

	}
}
