﻿#region BSD License
/*
 * 
 *  New BSD 3-Clause License (https://github.com/Krypton-Suite/Standard-Toolkit/blob/master/LICENSE)
 *  Modifications by Peter Wagner(aka Wagnerp) & Simon Coghlan(aka Smurf-IV), et al. 2023 - 2023. All rights reserved. 
 *  
 */
#endregion


namespace Krypton.Toolkit
{
    /// <summary>Allows the user to change themes using a <see cref="KryptonListBox"/>.</summary>
    /// <seealso cref="KryptonListBox" />
    public class KryptonThemeListBox : KryptonListBox
    {
        #region Instance Fields

        private int _selectedThemeIndex;

        #endregion

        #region Public

        /// <summary>
        /// Gets and sets the ThemeSelectedIndex.
        /// </summary>
        [Category(@"Visuals")]
        [Description(@"Theme Selected Index. (Default = `Office 365 - Blue`)")]
        [DefaultValue(33)]
        public int ThemeSelectedIndex
        {
            get => _selectedThemeIndex;

            set => SelectedIndex = value;
        }

        private void ResetThemeSelectedIndex() => _selectedThemeIndex = 33;

        private bool ShouldSerializeThemeSelectedIndex() => _selectedThemeIndex != 33;

        /// <summary>
        /// Gets and sets the ThemeSelectedIndex.
        /// </summary>
        [Category(@"Visuals")]
        [Description(@"Custom Theme to use when `Custom` is selected")]
        [DefaultValue(null)]
        public KryptonCustomPaletteBase? KryptonCustomPalette { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public KryptonManager Manager
        {
            get;

        } = new KryptonManager();

        #endregion

        #region Identity

        /// <summary>Initializes a new instance of the <see cref="KryptonThemeListBox" /> class.</summary>
        public KryptonThemeListBox()
        {
            foreach (var kvp in PaletteModeStrings.SupportedThemesMap)
            {
                Items.Add(kvp.Key);
            }
            Text = ThemeManager.ReturnPaletteModeAsString(PaletteMode.Microsoft365Blue);
            _selectedThemeIndex = SelectedIndex;
            Debug.Assert(_selectedThemeIndex == 33, "Microsoft365Blue needs to be at the 33rd index for backward compatibility");
        }

        #endregion

        #region Implementation

        /// <summary>Returns the palette mode.</summary>
        public PaletteMode ReturnPaletteMode() => Manager.GlobalPaletteMode;

        #endregion

        #region Protected

        /// <inheritdoc />
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            SelectedIndex = _selectedThemeIndex;
        }

        /// <inheritdoc />
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            ThemeManager.ApplyTheme(GetItemText(SelectedItem), Manager);

            ThemeSelectedIndex = SelectedIndex;

            base.OnSelectedIndexChanged(e);

            if ((ThemeManager.GetThemeManagerMode(GetItemText(SelectedItem)) == PaletteMode.Custom) && (KryptonCustomPalette != null))
            {
                Manager.GlobalPalette = KryptonCustomPalette;
            }
        }

        #endregion

        #region Removed Designer Visibility

        /// <summary>Gets and sets the text associated associated with the control.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DisallowNull]
        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        /// <summary>Gets the items of the KryptonListBox.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ListBox.ObjectCollection Items => base.Items;

        #endregion
    }
}