using System.Collections.ObjectModel;
using System.Collections.Generic;
using AvaloniaAppUsingHost.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaAppUsingHost.ViewModels.TreeView;

/// <summary>
/// Displays a small hierarchical data set in a tree view.
/// </summary>
public sealed partial class TreeViewPageViewModel : ScreenPage
{
    /// <summary>
    /// Gets the root nodes displayed by the tree.
    /// </summary>
    public ObservableCollection<TreeNodeViewModel> Nodes { get; } =
    [
        new TreeNodeViewModel("Projects", "Folder",
        [
            new TreeNodeViewModel("Avalonia host", "Table"),
        ]),
        new TreeNodeViewModel("Resources", "Folder",
        [
            new TreeNodeViewModel("Styles", "Table"),
            new TreeNodeViewModel("Assets", "Table")
        ])
    ];

    /// <summary>
    /// Gets or sets the node currently selected in the tree.
    /// </summary>
    [ObservableProperty]
    public partial TreeNodeViewModel? SelectedNode { get; set; }

    /// <summary>
    /// Gets the title displayed in the tab view.
    /// </summary>
    public override string Title => "Tree view";
}

public interface ITreeNode
{
    /// <summary>
    /// Gets the text displayed for this item.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the identifier of the icon displayed beside this item.
    /// </summary>
    string IconId { get; }

    IReadOnlyList<ITreeNode> Children { get; }
}


/// <summary>
/// Represents one node in the sample tree and its optional child nodes.
/// </summary>
public sealed class TreeNodeViewModel : ObservableObject, ITreeNode
{
    /// <summary>
    /// Initializes a tree node with a display name and optional children.
    /// </summary>
    public TreeNodeViewModel(string name, string iconId, IEnumerable<TreeNodeViewModel>? children = null)
    {
        Name = name;
        IconId = iconId;
        Children = children is null ? [] : new ObservableCollection<TreeNodeViewModel>(children);
    }

    /// <summary>
    /// Gets the text displayed for this node.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the identifier of the icon displayed beside this node.
    /// </summary>
    public string IconId { get; }

    IReadOnlyList<ITreeNode> ITreeNode.Children => Children;
    
    /// <summary>
    /// Gets the child nodes nested beneath this node.
    /// </summary>
    public ObservableCollection<TreeNodeViewModel> Children { get; }
}
