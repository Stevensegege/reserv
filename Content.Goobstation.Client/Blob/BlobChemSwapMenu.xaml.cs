// SPDX-FileCopyrightText: 2024 Aiden <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Fishbait <Fishbait@git.ml>
// SPDX-FileCopyrightText: 2024 fishbait <gnesse@gmail.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Misandry <mary@thughunt.ing>
// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Content.Client.Stylesheets;
using Content.Goobstation.Shared.Blob.Components;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Client.Blob;

[GenerateTypedNameReferences]
public sealed partial class BlobChemSwapMenu : DefaultWindow
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    private readonly SpriteSystem _sprite;
    public event Action<BlobChemType>? OnIdSelected;

    private Shared.Blob.BlobChemColors _possibleChems = new();
    private BlobChemType _selectedId;

    public BlobChemSwapMenu()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);
        _sprite = _entityManager.System<SpriteSystem>();
    }

    public void UpdateState(Shared.Blob.BlobChemColors chemList, BlobChemType selectedChem)
    {
        _possibleChems = chemList;
        _selectedId = selectedChem;
        UpdateGrid();
    }

    protected override void EnteredTree()
    {
        UpdateGrid();
    }

    protected override void ExitedTree()
    {
        ClearGrid();
    }

    [ValidatePrototypeId<EntityPrototype>]
    private const string NormalBlobTile = "NormalBlobTile";

    private void UpdateGrid()
    {
        ClearGrid();

        var group = new ButtonGroup();

        foreach (var (blobChem,blobColor) in _possibleChems)
        {
            if (!_prototypeManager.TryIndex(NormalBlobTile, out EntityPrototype? proto))
                continue;

            var button = new Button
            {
                MinSize = new Vector2(64, 64),
                HorizontalExpand = true,
                Group = group,
                StyleClasses = {StyleBase.ButtonSquare},
                ToggleMode = true,
                Pressed = _selectedId == blobChem,
                ToolTip = Loc.GetString($"blob-chem-{blobChem.ToString().ToLower()}-info"),
                TooltipDelay = 0.01f,
            };
            button.OnPressed += _ => OnIdSelected?.Invoke(blobChem);
            Grid.AddChild(button);

            var texture = _sprite.GetPrototypeIcon(proto);
            button.AddChild(new TextureRect
            {
                Stretch = TextureRect.StretchMode.KeepAspectCentered,
                Modulate = blobColor,
                Texture = texture.Default,
            });
        }
    }

    private void ClearGrid()
    {
        Grid.RemoveAllChildren();
    }
}