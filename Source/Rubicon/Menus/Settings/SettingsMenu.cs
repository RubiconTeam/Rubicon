using System.Reflection;
using Rubicon.Menus;
using Rubicon.Data.Settings;
using Rubicon.Data.Settings.Attributes;

namespace Rubicon.Data.Settings;
public partial class SettingsMenu : BaseMenu
{
    [NodePath("Main/Sidebar/AnimationPlayer")] private AnimationPlayer _sidebarAnimationPlayer;
    [NodePath("Main/Sidebar")] private Control _sidebar;
    [NodePath("Main/UI/SectionButtons/PanelContainer/VBoxContainer")] private Control _sectionButtonContainer;
    [NodePath("Main/UI/SectionButtons/PanelContainer/VBoxContainer/Section")] private Control _sectionButtonTemplate;

    public override void _Ready()
    {
        this.OnReady();
        base._Ready();

        _sidebar.MouseEntered += () => _sidebarAnimationPlayer.Play("Enter");
        _sidebar.MouseExited += () => _sidebarAnimationPlayer.Play("Leave");

        // Hide the placeholder button
        _sectionButtonTemplate.Visible = false;

        foreach (var prop in typeof(UserSettingsData).GetProperties())
        {
            var attribute = prop.PropertyType.GetCustomAttribute<RubiconSettingsSectionAttribute>();
            if (attribute != null && attribute.GenerateInMenu) CreateSectionButton(attribute.Name, attribute.Icon);
        }
    }

    private void CreateSectionButton(string sectionName, Texture2D icon)
    {
        var buttonInstance = _sectionButtonTemplate.Duplicate() as Control;
        if (buttonInstance != null)
        {
            buttonInstance.Name = sectionName;

            var textureRect = buttonInstance.GetNode<TextureRect>("Icon");
            var label = buttonInstance.GetNode<Label>("Text");

            textureRect.Texture = icon;
            label.Text = sectionName;
            label.Name = sectionName;

            _sectionButtonContainer.AddChild(buttonInstance);
        }
    }
}