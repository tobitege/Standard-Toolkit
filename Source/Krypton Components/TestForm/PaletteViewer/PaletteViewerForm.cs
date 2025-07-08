using System;
using System.Windows.Forms;
using Krypton.Toolkit;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TestForm
{
    public partial class PaletteViewerForm : KryptonForm
    {
        private readonly System.Collections.Generic.List<Krypton.Toolkit.PaletteBase> _palettes = new System.Collections.Generic.List<Krypton.Toolkit.PaletteBase>();
        private System.Collections.Generic.List<System.Reflection.MethodInfo> _apiCalls;
        private System.Collections.Generic.Dictionary<System.Reflection.MemberInfo, string> _methodEnumMapping;
        private Krypton.Toolkit.SchemeBaseColors[] _enumValues;
        private System.Drawing.Color[] _baselineColors;
        private string _baselinePaletteName;
        private string _sourcePath;
        private bool _isSourceValid;
        private System.Threading.CancellationTokenSource _addAllCts;
        private int _highlightRowIndex = -1;
        private bool _bulkUpdating;
        private const int MinColumnWidth = 120;
        private readonly WindowStateStore _winStore;
        private readonly WindowStateInfo _winInfo;
        private static readonly IReadOnlyDictionary<string, Krypton.Toolkit.PaletteMode> DisplayToEnum = Krypton.Toolkit.PaletteModeStrings.SupportedThemesMap;
        private static readonly System.Collections.Generic.Dictionary<Krypton.Toolkit.PaletteMode, string> EnumToDisplay = new System.Collections.Generic.Dictionary<Krypton.Toolkit.PaletteMode, string>(System.Linq.Enumerable.ToDictionary(DisplayToEnum, kv => kv.Value, kv => kv.Key));
        private static bool IsSparkleDisplay(string display) => display.StartsWith("Sparkle", System.StringComparison.OrdinalIgnoreCase);
        private static bool IsSparkleMode(Krypton.Toolkit.PaletteMode mode) => mode.ToString().StartsWith("Sparkle", System.StringComparison.OrdinalIgnoreCase);
        private static bool IsSparkleType(System.Type t) => t.Name.StartsWith("PaletteSparkle", System.StringComparison.OrdinalIgnoreCase);

        private static readonly Krypton.Toolkit.PaletteMode[] LegacyProfessionalModes = new[]
        {
            Krypton.Toolkit.PaletteMode.ProfessionalSystem,
            Krypton.Toolkit.PaletteMode.ProfessionalOffice2003
        };

        private static bool IsLegacyProfessionalMode(Krypton.Toolkit.PaletteMode mode) => System.Array.IndexOf(LegacyProfessionalModes, mode) >= 0;
        private static bool IsLegacyProfessionalDisplay(string display) => DisplayToEnum.TryGetValue(display, out var m) && IsLegacyProfessionalMode(m);
        private static bool IsLegacyProfessionalType(System.Type t) => t.Name.StartsWith("PaletteProfessional", System.StringComparison.OrdinalIgnoreCase);

        private static bool IsProblematicBaseMethod(System.Reflection.MethodInfo m)
        {
            if (m.DeclaringType == null)
            {
                return false;
            }

            string typeName = m.DeclaringType.Name;

            // Exclude any GetRibbonBackColor* method declared on a *Base palette class,
            // as many of these throw or show dialogs for unsupported styles.
            if (typeName.EndsWith("Base", System.StringComparison.Ordinal) &&
                m.Name.StartsWith("GetRibbonBackColor", System.StringComparison.Ordinal))
            {
                return true;
            }

            // Add other known problematic patterns here if discovered

            return false;
        }

        private readonly System.Collections.Generic.HashSet<System.Type> _processedBases = new System.Collections.Generic.HashSet<System.Type>();
        private Krypton.Toolkit.KryptonManager kryptonManager1;

        public PaletteViewerForm()
        {
            InitializeComponent();

            // load persisted window state
            _winStore = new WindowStateStore();
            _winInfo = _winStore.Load();

            PopulateThemeCombo(_winInfo?.LastTheme);

            _sourcePath = _winInfo?.SourcePath;
            _isSourceValid = IsValidSourcePath(_sourcePath);
            if (this.textSourcePath != null)
            {
                this.textSourcePath.Text = _sourcePath ?? string.Empty;
            }

            var workArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;

            if (_winInfo != null)
            {
                StartPosition = FormStartPosition.Manual;
                Left = _winInfo.Left;
                Top = _winInfo.Top;

                var screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point(_winInfo.Left, _winInfo.Top));
                var wa = screen.WorkingArea;
                Width = System.Math.Min(_winInfo.Width, wa.Width);
                Height = System.Math.Min(_winInfo.Height, wa.Height);

                var st = _winInfo.State == FormWindowState.Minimized ? FormWindowState.Normal : _winInfo.State;
                WindowState = st;
            }
            else
            {
                this.Size = new System.Drawing.Size(workArea.Width / 2, workArea.Height / 2);
                this.Location = new System.Drawing.Point((workArea.Width - this.Width) / 2, (workArea.Height - this.Height) / 2);
            }

            this.FormClosing += MainForm_FormClosing;

            UpdateStatus("Ready");

            UpdateUIState();
        }

        /// <summary>
        /// Determines whether the provided toolkit root path is structurally valid.
        /// </summary>
        private static bool IsValidSourcePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                return false;
            }

            // Must contain top-level "Source" folder.
            var sourceDir = Path.Combine(path, "Source");
            if (!Directory.Exists(sourceDir))
            {
                return false;
            }

            // Required Enumerations folder and Microsoft 365 folder.
            var enumDir = Path.Combine(sourceDir, "Krypton Components", "Krypton.Toolkit", "Palette Builtin", "Enumerations");
            var m365Dir = Path.Combine(sourceDir, "Krypton Components", "Krypton.Toolkit", "Palette Builtin", "Microsoft 365");
            if (!Directory.Exists(enumDir) || !Directory.Exists(m365Dir))
            {
                return false;
            }

            // Required file PaletteEnumerations.cs inside Enumerations folder.
            var enumFile = Path.Combine(enumDir, "PaletteEnumerations.cs");
            if (!File.Exists(enumFile))
            {
                return false;
            }

            return true;
        }

        public void AttachKryptonManager(Krypton.Toolkit.KryptonManager manager)
        {
            kryptonManager1 = manager;
            UpdateThemeSwitcher();
        }

        private void UpdateThemeSwitcher()
        {
            if (kryptonManager1 == null || kryptonThemeComboBox == null)
            {
                return;
            }

            if (kryptonManager1.GlobalPaletteMode != Krypton.Toolkit.PaletteMode.Custom && EnumToDisplay.TryGetValue(kryptonManager1.GlobalPaletteMode, out string disp))
            {
                kryptonThemeComboBox.SelectedItem = disp;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateThemeSwitcher();
        }

        private void LoadApiCalls()
        {
            if (_apiCalls != null)
            {
                return;
            }

            var baseType = typeof(Krypton.Toolkit.PaletteMicrosoft365Base);
            var methods = baseType.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            var list = new System.Collections.Generic.List<System.Reflection.MethodInfo>();

            foreach (var m in methods)
            {
                if (m.IsSpecialName || IsProblematicBaseMethod(m))
                {
                    continue;
                }

                if (m.ReturnType != typeof(System.Drawing.Color))
                {
                    continue;
                }

                // Skip methods with ref/out parameters or open generics
                bool skip = false;
                foreach (var p in m.GetParameters())
                {
                    if (p.IsOut || p.ParameterType.IsByRef)
                    {
                        skip = true;
                        break;
                    }
                }

                if (skip)
                {
                    continue;
                }

                list.Add(m);
            }

            _apiCalls = list;

            // prepare enum values array
            _enumValues = (Krypton.Toolkit.SchemeBaseColors[])System.Enum.GetValues(typeof(Krypton.Toolkit.SchemeBaseColors));

            BuildBaseRows();
        }

        private void BuildBaseRows()
        {
            this.dataGridViewPalette.Columns.Clear();
            this.dataGridViewPalette.Rows.Clear();

            // Column 0: Enum Index
            var colIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colIndex.HeaderText = "#";
            colIndex.Name = "colIndex";
            colIndex.ReadOnly = true;
            colIndex.Frozen = true;
            colIndex.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            colIndex.MinimumWidth = 50;
            colIndex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewPalette.Columns.Add(colIndex);

            // Column 1: Enum Name
            var colEnum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colEnum.HeaderText = "SchemeBaseColors";
            colEnum.Name = "colEnum";
            colEnum.ReadOnly = true;
            colEnum.Frozen = true;
            colEnum.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewPalette.Columns.Add(colEnum);

            // Column 2: API Call(s)
            var colApi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colApi.HeaderText = "API Call(s)";
            colApi.Name = "colApi";
            colApi.ReadOnly = true;
            colApi.MinimumWidth = 150;
            colApi.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            colApi.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colApi.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            colApi.Frozen = true;
            this.dataGridViewPalette.Columns.Add(colApi);

            if (_enumValues == null)
            {
                return;
            }

            for (int idx = 0; idx < _enumValues.Length; idx++)
            {
                int rowIndex = this.dataGridViewPalette.Rows.Add();
                var row = this.dataGridViewPalette.Rows[rowIndex];
                row.Cells[0].Value = idx; // index column
                row.Cells[1].Value = _enumValues[idx].ToString();
                row.Cells[2].Value = string.Empty; // API mapping
            }

            // Auto-resize columns once, then allow manual resize
            this.dataGridViewPalette.AutoResizeColumns(System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells);
            foreach (System.Windows.Forms.DataGridViewColumn col in this.dataGridViewPalette.Columns)
            {
                if (col.Name != "colApi")
                {
                    col.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
                }
            }

            UpdateUIState();
        }

        private void AdjustRowHeights()
        {
            const int padding = 4;

            int singleLineHeight = 22;

            foreach (System.Windows.Forms.DataGridViewRow row in this.dataGridViewPalette.Rows)
            {
                if (row.IsNewRow) continue;

                var cell = row.Cells.Count > 2 ? row.Cells[2] : null; // API column
                int lineCount = 1;
                if (cell?.Value is string txt && txt.Length > 0)
                {
                    lineCount = txt.Split('\n').Length;
                }

                row.Height = (singleLineHeight * lineCount) + padding;
            }
        }

        private void BuildMethodEnumMapping(Krypton.Toolkit.PaletteBase palette)
        {
            if (palette == null)
            {
                return;
            }

            var mapping = new System.Collections.Generic.Dictionary<System.Reflection.MemberInfo, string>();

            System.Drawing.Color[] colorArray = TryGetPaletteColors(palette);

            if (colorArray == null || colorArray.Length == 0)
            {
                _methodEnumMapping = mapping;
                return;
            }

            // 1. Try to map public color-returning methods (including overrides)
            var paletteMethods = palette.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var m in paletteMethods)
            {
                if (m.IsSpecialName || m.ReturnType != typeof(System.Drawing.Color) || (IsProblematicBaseMethod(m)))
                {
                    continue;
                }

                // Skip methods with ref/out parameters
                bool skip = false;
                foreach (var p in m.GetParameters())
                {
                    if (p.IsOut || p.ParameterType.IsByRef)
                    {
                        skip = true;
                        break;
                    }
                }
                if (skip)
                {
                    continue;
                }

                System.Drawing.Color resultColor;
                try
                {
                    var parameters = m.GetParameters();
                    object[] args = new object[parameters.Length];
                    // Use the first enum value or default struct instance as placeholder argument
                    for (int idx = 0; idx < parameters.Length; idx++)
                    {
                        var pt = parameters[idx].ParameterType;
                        if (pt.IsEnum)
                        {
                            args[idx] = System.Enum.GetValues(pt).GetValue(0);
                        }
                        else if (pt.IsValueType)
                        {
                            args[idx] = System.Activator.CreateInstance(pt);
                        }
                        else
                        {
                            args[idx] = null;
                        }
                    }

                    resultColor = (System.Drawing.Color)m.Invoke(palette, args);
                }
                catch (System.Reflection.TargetInvocationException tie)
                {
                    // Ignore known palette issues that manifest as exceptions
                    if (tie.InnerException is System.NotImplementedException ||
                        tie.InnerException is System.ArgumentOutOfRangeException ||
                        tie.InnerException is System.IndexOutOfRangeException)
                    {
                        continue;
                    }

                    continue;
                }

                // Match against array to find enum slot
                for (int i = 0; i < colorArray.Length; i++)
                {
                    if (colorArray[i].ToArgb() == resultColor.ToArgb())
                    {
                        if (System.Enum.IsDefined(typeof(Krypton.Toolkit.SchemeBaseColors), i))
                        {
                            var enumName = ((Krypton.Toolkit.SchemeBaseColors)i).ToString();
                            mapping[m] = enumName;
                        }
                        break;
                    }
                }
            }

            // Map ColorTable properties as well
            if (palette.ColorTable != null)
            {
                var ctProps = palette.ColorTable.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                foreach (var prop in ctProps)
                {
                    if (prop.PropertyType != typeof(System.Drawing.Color) || prop.GetIndexParameters().Length > 0)
                    {
                        continue;
                    }

                    System.Drawing.Color c;
                    try
                    {
                        c = (System.Drawing.Color)prop.GetValue(palette.ColorTable);
                    }
                    catch
                    {
                        continue;
                    }

                    for (int i = 0; i < colorArray.Length; i++)
                    {
                        if (colorArray[i].ToArgb() == c.ToArgb())
                        {
                            if (System.Enum.IsDefined(typeof(Krypton.Toolkit.SchemeBaseColors), i))
                            {
                                var enumName = ((Krypton.Toolkit.SchemeBaseColors)i).ToString();
                                mapping[prop] = enumName;
                            }
                            break;
                        }
                    }
                }
            }

            MergeEnumMethodMapping(mapping);

            // Update grid mapping column
            if (_enumValues != null)
            {
                // create reverse map enumName -> list of methods
                var enumMethodMap = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>();
                foreach (var kvp in mapping)
                {
                    var enumName = kvp.Value;
                    if (!enumMethodMap.TryGetValue(enumName, out var list))
                    {
                        list = new System.Collections.Generic.List<string>();
                        enumMethodMap.Add(enumName, list);
                    }
                    list.Add(kvp.Key.Name);
                }

                for (int i = 0; i < _enumValues.Length; i++)
                {
                    var enName = _enumValues[i].ToString();
                    if (enumMethodMap.TryGetValue(enName, out var mlist))
                    {
                        var joined = string.Join("\n", mlist);
                        var apiCell = this.dataGridViewPalette.Rows[i].Cells[2];
                        apiCell.Value = joined;
                        apiCell.Tag = mlist.Count > 1;
                    }
                }
            }

            UpdateUIState();
        }

        private void AddPalette(Krypton.Toolkit.PaletteBase palette)
        {
            if (palette == null)
            {
                return;
            }

            var modeForPalette = Krypton.Toolkit.KryptonManager.GetModeForPalette(palette);
            if (IsSparkleMode(modeForPalette) || IsSparkleType(palette.GetType()) || IsLegacyProfessionalMode(modeForPalette) || IsLegacyProfessionalType(palette.GetType()))
            {
                return; // skip unsupported palette types
            }

            _palettes.Add(palette);

            // Build/merge mapping for each unique palette base (class that owns the _ribbonColors field)
            var baseOwner = GetRibbonColorsOwner(palette.GetType());
            if (baseOwner != null && !_processedBases.Contains(baseOwner))
            {
                BuildMethodEnumMapping(palette);
                _processedBases.Add(baseOwner);
            }

            // Add column
            var rawHeader = modeForPalette != Krypton.Toolkit.PaletteMode.Custom ? GetDisplayName(modeForPalette) : palette.GetType().Name;
            var baseHeader = BreakHeader(rawHeader);

            var col = new System.Windows.Forms.DataGridViewTextBoxColumn();
            col.HeaderText = baseHeader;
            col.Name = palette.GetType().FullName;
            col.ReadOnly = false;
            col.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            col.HeaderCell.Style.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            col.HeaderCell.Style.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewPalette.Columns.Add(col);

            int paletteColumnIndex = this.dataGridViewPalette.Columns.Count - 1;

            // populate enum rows with color values

            // fetch palette color array
            System.Drawing.Color[] paletteColors = TryGetPaletteColors(palette);

            // Assign baseline if not yet set
            bool isBaseline = false;
            if (_baselineColors == null && paletteColors != null)
            {
                _baselineColors = paletteColors;
                _baselinePaletteName = palette.GetType().Name;
                isBaseline = true;
            }

            int missingCount = 0;
            bool sparsePalette = paletteColors == null || paletteColors.Length < _enumValues.Length / 3; // heuristics for incompatible palettes

            string headerSuffix = string.Empty;

            for (int i = 0; i < _enumValues.Length; i++)
            {
                var row = this.dataGridViewPalette.Rows[i];

                bool indexPresent = paletteColors != null && i < paletteColors.Length;
                System.Drawing.Color color = indexPresent ? paletteColors[i] : System.Drawing.Color.Transparent;
                if (!indexPresent)
                {
                    // count missing only if palette is expected to support full scheme
                    if (!sparsePalette)
                    {
                        missingCount++;
                        if (string.IsNullOrEmpty(row.Cells[paletteColumnIndex].ErrorText))
                        {
                            row.Cells[paletteColumnIndex].ErrorText = "Missing index";
                        }
                    }
                }
                else if (color.A == 0)
                {
                    // Intentionally empty – mark as warning (italic text) but no error icon
                    string label = color == System.Drawing.Color.Transparent ? "Transparent" : "EMPTY";
                    row.Cells[paletteColumnIndex].Value = label;
                    row.Cells[paletteColumnIndex].Style.Font = new System.Drawing.Font(this.Font, System.Drawing.FontStyle.Italic);
                }

                var cell = row.Cells[paletteColumnIndex];
                var textColor = ContrastColor(color);
                cell.Style.BackColor = color;
                cell.Style.ForeColor = textColor;
                cell.Style.SelectionBackColor = AdjustSelectionBack(color);
                cell.Style.SelectionForeColor = textColor;
                cell.Value = "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");

                cell.ToolTipText = palette.GetType().Name + " - " + _enumValues[i];

                // Legacy fallback removed – handled below for missing or intentional transparent values
            }

            // Static source check for array vs enum
            if (!string.IsNullOrWhiteSpace(_sourcePath))
            {
                var issues = Classes.ThemeArrayInspector.GetIssues(palette.GetType(), _sourcePath);
                if (issues != null && !issues.IsClean)
                {
                    foreach (int idx in issues.MissingIndices)
                    {
                        if (idx >= 0 && idx < this.dataGridViewPalette.Rows.Count)
                        {
                            var c = this.dataGridViewPalette.Rows[idx].Cells[paletteColumnIndex];
                            if (string.IsNullOrEmpty(c.ErrorText)) c.ErrorText = "Missing value";
                            if (!string.IsNullOrEmpty(c.ErrorText) && (c.ToolTipText == null || !c.ToolTipText.Contains(c.ErrorText)))
                            {
                                c.ToolTipText += " : " + c.ErrorText;
                            }
                        }
                    }

                    foreach (int idx in issues.UnlabelledIndices)
                    {
                        if (idx >= 0 && idx < this.dataGridViewPalette.Rows.Count)
                        {
                            var c = this.dataGridViewPalette.Rows[idx].Cells[paletteColumnIndex];
                            if (string.IsNullOrEmpty(c.ErrorText)) c.ErrorText = "Unmarked value";
                            if (!string.IsNullOrEmpty(c.ErrorText) && (c.ToolTipText == null || !c.ToolTipText.Contains(c.ErrorText)))
                            {
                                c.ToolTipText += " : " + c.ErrorText;
                            }
                        }
                    }

                    foreach (int idx in issues.OutOfOrderIndices)
                    {
                        if (idx >= 0 && idx < this.dataGridViewPalette.Rows.Count)
                        {
                            var c = this.dataGridViewPalette.Rows[idx].Cells[paletteColumnIndex];
                            if (string.IsNullOrEmpty(c.ErrorText)) c.ErrorText = "Out-of-order";
                            if (!string.IsNullOrEmpty(c.ErrorText) && (c.ToolTipText == null || !c.ToolTipText.Contains(c.ErrorText)))
                            {
                                c.ToolTipText += " : " + c.ErrorText;
                            }
                        }
                    }

                    foreach (int idx in issues.ExtraIndices)
                    {
                        if (idx >= 0 && idx < this.dataGridViewPalette.Rows.Count)
                        {
                            var c = this.dataGridViewPalette.Rows[idx].Cells[paletteColumnIndex];
                            if (string.IsNullOrEmpty(c.ErrorText)) c.ErrorText = "Extra entry";
                            if (!string.IsNullOrEmpty(c.ErrorText) && (c.ToolTipText == null || !c.ToolTipText.Contains(c.ErrorText)))
                            {
                                c.ToolTipText += " : " + c.ErrorText;
                            }
                        }
                    }

                    headerSuffix = $"\nΔ {issues.MissingCount}/{issues.UnlabelledCount}/{issues.OutOfOrderCount}/{issues.ExtraCount}";
                }
            }

            // Update column header with summary
            var colHeader = baseHeader;

            if (sparsePalette)
            {
                colHeader += "\n(incompatible)";
            }
            else if (missingCount > 0)
            {
                colHeader += $"\n⚠{missingCount} missing";
            }

            colHeader += headerSuffix;

            this.dataGridViewPalette.Columns[paletteColumnIndex].HeaderText = colHeader;
            this.dataGridViewPalette.Columns[paletteColumnIndex].HeaderCell.Style.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewPalette.Columns[paletteColumnIndex].HeaderCell.Style.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;

            // Recalculate header height to accommodate up to 3 lines
            this.dataGridViewPalette.AutoResizeColumnHeadersHeight();

            // Auto-size new column to contents once, then freeze
            int preferred = col.GetPreferredWidth(System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells, true);
            preferred = System.Math.Max(MinColumnWidth, System.Math.Min(preferred, 300));
            col.Width = preferred;
            col.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;

            // ensure new column is not sortable
            col.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;

            // After populating all rows for this palette, adjust row heights only if not in bulk update
            if (!_bulkUpdating)
            {
                AdjustRowHeights();
            }

            UpdateUIState();
        }

        private static System.Drawing.Color ContrastColor(System.Drawing.Color c)
        {
            // If fully transparent, fallback to default grid text colour (black)
            if (c.A == 0)
            {
                return System.Drawing.Color.Black;
            }

            // Convert sRGB to linear RGB
            double R = c.R / 255.0;
            double G = c.G / 255.0;
            double B = c.B / 255.0;

            R = R <= 0.03928 ? R / 12.92 : System.Math.Pow((R + 0.055) / 1.055, 2.4);
            G = G <= 0.03928 ? G / 12.92 : System.Math.Pow((G + 0.055) / 1.055, 2.4);
            B = B <= 0.03928 ? B / 12.92 : System.Math.Pow((B + 0.055) / 1.055, 2.4);

            // Calculate relative luminance
            double L = 0.2126 * R + 0.7152 * G + 0.0722 * B;

            // Choose foreground that meets WCAG 2.0 contrast ratio of at least 4.5:1
            // For white text contrast ratio is (1.0 + 0.05) / (L + 0.05)
            // For black text contrast ratio is (L + 0.05) / 0.05
            // White is better if background luminance <= 0.179
            return L <= 0.179 ? System.Drawing.Color.White : System.Drawing.Color.Black;
        }

        private static System.Drawing.Color AdjustSelectionBack(System.Drawing.Color c)
        {
            // If color is dark, lighten; if light, darken.
            double factor = 0.3; // 30% difference
            bool isLight = ((c.R * 299 + c.G * 587 + c.B * 114) / 1000) > 128;
            int r = isLight ? (int)(c.R * (1 - factor)) : (int)(c.R + (255 - c.R) * factor);
            int g = isLight ? (int)(c.G * (1 - factor)) : (int)(c.G + (255 - c.G) * factor);
            int b = isLight ? (int)(c.B * (1 - factor)) : (int)(c.B + (255 - c.B) * factor);
            return System.Drawing.Color.FromArgb(c.A, r, g, b);
        }

        private void BtnAddPalette_Click(object sender, System.EventArgs e)
        {
            this.LoadApiCalls();

            this.buttonAddPalette.Enabled = false;
            this.buttonRemovePalette.Enabled = false;
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            UpdateStatus("Adding palette...");
            System.Windows.Forms.Application.DoEvents();

            try
            {
                if (this.comboTheme.SelectedItem is string display && DisplayToEnum.TryGetValue(display, out Krypton.Toolkit.PaletteMode mode))
                {
                    var palette = Krypton.Toolkit.KryptonManager.GetPaletteForMode(mode);

                    if (!_palettes.Contains(palette))
                    {
                        AddPalette(palette);
                    }
                }
            }
            finally
            {
                UpdateStatus("Ready");
                this.Cursor = System.Windows.Forms.Cursors.Default;
                // State recalculated based on current validity
                UpdateUIState();
            }
        }

        private void BtnRemovePalette_Click(object sender, System.EventArgs e)
        {
            if (!(this.comboTheme.SelectedItem is string display) || !DisplayToEnum.TryGetValue(display, out Krypton.Toolkit.PaletteMode mode))
            {
                return; // nothing selected
            }

            var palette = _palettes.Find(p => KryptonManager.GetModeForPalette(p) == mode);
            if (palette == null)
            {
                return; // selected palette not loaded
            }

            // remove from list
            _palettes.Remove(palette);

            // find column
            var column = this.dataGridViewPalette.Columns.GetColumnCount(DataGridViewElementStates.Visible) > 0 ? this.dataGridViewPalette.Columns[palette.GetType().FullName] : null;
            if (column != null)
            {
                this.dataGridViewPalette.Columns.Remove(column);
            }

            UpdateUIState();
        }

        private void PopulateThemeCombo(string preferredDisplay = "")
        {
            foreach (var kvp in DisplayToEnum)
            {
                if (kvp.Value == Krypton.Toolkit.PaletteMode.Custom)
                {
                    continue;
                }

                if (IsSparkleDisplay(kvp.Key) || IsSparkleMode(kvp.Value))
                {
                    continue; // skip sparkle
                }

                if (IsLegacyProfessionalDisplay(kvp.Key) || IsLegacyProfessionalMode(kvp.Value))
                {
                    continue; // skip legacy professional themes
                }

                this.comboTheme.Items.Add(kvp.Key);
            }

            if (!string.IsNullOrEmpty(preferredDisplay) && this.comboTheme.Items.Contains(preferredDisplay))
            {
                this.comboTheme.SelectedItem = preferredDisplay;
            }
        }

        private void ComboTheme_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            UpdateUIState();
        }

        private async void BtnAddAll_Click(object sender, System.EventArgs e)
        {
            // Ensure base rows and enum array are ready
            this.LoadApiCalls();

            if (_addAllCts != null)
            {
                return; // already running
            }

            _addAllCts = new System.Threading.CancellationTokenSource();
            var token = _addAllCts.Token;

            this.buttonAddAll.Enabled = false;
            this.buttonAddPalette.Enabled = false;
            this.buttonRemovePalette.Enabled = false;
            this.comboTheme.Enabled = false;
            this.buttonCancel.Visible = true;

            UpdateStatus("Adding all palettes...");

            // Begin bulk update
            _bulkUpdating = true;
            this.dataGridViewPalette.SuspendLayout();

            try
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    var modes = (Krypton.Toolkit.PaletteMode[])System.Enum.GetValues(typeof(Krypton.Toolkit.PaletteMode));
                    foreach (var mode in modes)
                    {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        if (mode == Krypton.Toolkit.PaletteMode.Custom || IsSparkleMode(mode) || IsLegacyProfessionalMode(mode))
                        {
                            continue; // skip unsupported modes
                        }

                        var palette = Krypton.Toolkit.KryptonManager.GetPaletteForMode(mode);

                        this.Invoke((System.Action)(() =>
                        {
                            if (!_palettes.Contains(palette))
                            {
                                AddPalette(palette);
                                UpdateStatus($"Added {mode}...");
                            }
                        }));
                    }
                }, token);
            }
            catch (System.OperationCanceledException)
            {
                UpdateStatus("Add all cancelled.");
            }
            finally
            {
                // End bulk update
                _bulkUpdating = false;
                this.dataGridViewPalette.ResumeLayout();

                // Perform final row-height adjustment once after bulk add completes
                AdjustRowHeights();

                this.buttonAddAll.Enabled = true;
                this.buttonAddPalette.Enabled = true;
                this.buttonRemovePalette.Enabled = true;
                this.comboTheme.Enabled = true;
                this.buttonCancel.Visible = false;

                _addAllCts.Dispose();
                _addAllCts = null;

                UpdateStatus("Ready");

                UpdateUIState();
            }
        }

        private void BtnCancel_Click(object sender, System.EventArgs e)
        {
            _addAllCts?.Cancel();
        }

        private void UpdateStatus(string message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((System.Action)(() => UpdateStatus(message)));
                return;
            }

            this.statusLabel.Text = message;
        }

        private void DataGridViewPalette_ColumnAdded(object sender, System.Windows.Forms.DataGridViewColumnEventArgs e)
        {
            // auto-size once then freeze
            int preferred = e.Column.GetPreferredWidth(System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells, true);
            preferred = System.Math.Max(MinColumnWidth, System.Math.Min(preferred, 300));
            e.Column.Width = preferred;
            if (e.Column.Index != 2)
            {
                e.Column.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            }
            else
            {
                e.Column.Frozen = true;
            }
            e.Column.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
        }

        private void DataGridViewPalette_ColumnHeaderMouseClick(object sender, System.Windows.Forms.DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 3)
            {
                return;
            }

            var column = this.dataGridViewPalette.Columns[e.ColumnIndex];
            var palette = _palettes.Find(p => p.GetType().FullName == column.Name);

            // New guard: do not activate palettes that still have missing enum values
            if (palette != null && !string.IsNullOrWhiteSpace(_sourcePath))
            {
                var issues = Classes.ThemeArrayInspector.GetIssues(palette.GetType(), _sourcePath);
                if (issues != null && issues.MissingCount > 0)
                {
                    UpdateStatus($"Cannot activate {palette.GetType().Name}: {issues.MissingCount} missing enum colours");
                    return;
                }
            }

            if (palette != null)
            {
                var mode = Krypton.Toolkit.KryptonManager.GetModeForPalette(palette);
                if (mode != Krypton.Toolkit.PaletteMode.Custom)
                {
                    kryptonManager1.GlobalPaletteMode = mode;
                }
                else if (palette is Krypton.Toolkit.KryptonCustomPaletteBase cp)
                {
                    kryptonManager1.GlobalPaletteMode = Krypton.Toolkit.PaletteMode.Custom;
                    kryptonManager1.GlobalCustomPalette = cp;
                }

                string headerFlat = column.HeaderText.Replace("\n", " ").Replace("\r", " ");
                UpdateStatus($"Activated {headerFlat}");
            }
        }

        private void BtnClear_Click(object sender, System.EventArgs e)
        {
            this._palettes.Clear();
            _baselineColors = null;
            _baselinePaletteName = null;
            _processedBases.Clear();
            _methodEnumMapping = null;

            BuildBaseRows();

            UpdateStatus("Cleared palettes");

            UpdateUIState();
        }

        private static string BreakHeader(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                char ch = text[i];
                char prev = text[i - 1];
                if (char.IsUpper(ch) && !char.IsUpper(prev))
                {
                    sb.Append('\n');
                }
                sb.Append(ch);
            }
            return sb.ToString();
        }

        private string GetDisplayName(Krypton.Toolkit.PaletteMode mode)
        {
            if (EnumToDisplay.TryGetValue(mode, out string display))
            {
                return display;
            }
            return mode.ToString();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // determine bounds depending on state
            var stateForSave = this.WindowState;
            var bounds = stateForSave == FormWindowState.Normal ? this.Bounds : this.RestoreBounds;

            var ws = new WindowStateInfo
            {
                Left = bounds.Left,
                Top = bounds.Top,
                Width = bounds.Width,
                Height = bounds.Height,
                State = stateForSave,
                LastTheme = this.comboTheme.SelectedItem as string,
                SourcePath = _sourcePath
            };

            _winStore.Save(ws);
        }

        private void DataGridViewPalette_CurrentCellChanged(object sender, System.EventArgs e)
        {
            if (_bulkUpdating || this.dataGridViewPalette.CurrentCell == null)
            {
                return;
            }

            int newIndex = this.dataGridViewPalette.CurrentCell.RowIndex;
            if (newIndex == _highlightRowIndex)
            {
                return;
            }

            int old = _highlightRowIndex;
            _highlightRowIndex = newIndex;

            if (old >= 0 && old < this.dataGridViewPalette.Rows.Count)
            {
                this.dataGridViewPalette.InvalidateRow(old);
            }
            if (newIndex >= 0 && newIndex < this.dataGridViewPalette.Rows.Count)
            {
                this.dataGridViewPalette.InvalidateRow(newIndex);
            }
        }

        private void DataGridViewPalette_Paint(object sender, PaintEventArgs e)
        {
            if (_bulkUpdating)
            {
                return;
            }

            if (_highlightRowIndex < 0 || _highlightRowIndex >= this.dataGridViewPalette.RowCount)
            {
                return;
            }

            var rowRect = this.dataGridViewPalette.GetRowDisplayRectangle(_highlightRowIndex, true);
            if (rowRect.Width <= 0 || rowRect.Height <= 0)
            {
                return; // row not visible
            }

            using (var pen = new System.Drawing.Pen(System.Drawing.Color.DarkOrange, 3))
            {
                rowRect.Width -= 1;
                rowRect.Height -= 1;
                e.Graphics.DrawRectangle(pen, rowRect);
            }
        }

        private void SaveCsv_Click(object sender, EventArgs e) => SaveGrid("csv");
        private void SaveXml_Click(object sender, EventArgs e) => SaveGrid("xml");

        private void SaveGrid(string format)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = $"{format.ToUpper()} files|*.{format}|All files|*.*";
                sfd.DefaultExt = format;
                if (sfd.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    switch (format)
                    {
                        case "csv":
                            File.WriteAllText(sfd.FileName, ExportCsv());
                            break;
                        case "xml":
                            ExportXml(sfd.FileName);
                            break;
                    }
                    UpdateStatus($"Saved {Path.GetFileName(sfd.FileName)}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string ExportCsv() => Classes.Exporter.ToCsv(this.dataGridViewPalette);

        private void ExportXml(string fileName) => Classes.Exporter.ToXml(this.dataGridViewPalette, fileName);

        private void UpdateUIState()
        {
            bool validSource = _isSourceValid;

            bool hasData = this.dataGridViewPalette.Columns.Count > 3 && this.dataGridViewPalette.Rows.Count > 0;
            this.buttonSave.Enabled = hasData;

            bool hasPalettes = _palettes.Count > 0;

            bool canRemove = false;
            bool canAdd = false;

            if (this.comboTheme.SelectedItem is string selDisplay && DisplayToEnum.TryGetValue(selDisplay, out PaletteMode selMode))
            {
                canRemove = _palettes.Exists(p => KryptonManager.GetModeForPalette(p) == selMode);
                canAdd = !canRemove;
            }

            this.buttonRemovePalette.Enabled = canRemove;
            this.buttonAddPalette.Enabled = canAdd && validSource;
            this.buttonClear.Enabled = hasPalettes;

            // AddAll button if exists
            if (this.buttonAddAll != null)
            {
                bool anyRemaining = false;
                foreach (var kv in DisplayToEnum)
                {
                    if (IsSparkleDisplay(kv.Key) || IsSparkleMode(kv.Value) || IsLegacyProfessionalDisplay(kv.Key) || IsLegacyProfessionalMode(kv.Value))
                    {
                        continue; // skip
                    }

                    if (!_palettes.Exists(p => KryptonManager.GetModeForPalette(p) == kv.Value))
                    {
                        anyRemaining = true;
                        break;
                    }
                }
                this.buttonAddAll.Enabled = validSource && anyRemaining;
            }

            if (this.buttonClearSource != null)
            {
                this.buttonClearSource.Enabled = !string.IsNullOrWhiteSpace(_sourcePath);
            }

            // Update source-required hint visibility
            if (this.labelSourceRequired != null)
            {
                this.labelSourceRequired.Visible = !validSource;
            }
        }

        private void DataGridViewPalette_SelectionChanged(object sender, EventArgs e)
        {
            UpdateUIState();
        }

        private void ButtonSave_Click(object sender, System.EventArgs e)
        {
            string format = "csv";
            if (this.comboSaveFormat.SelectedItem is string sel)
            {
                format = sel.ToLower();
            }

            SaveGrid(format);
        }

        private void DataGridViewPalette_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBoxBase tb)
            {
                tb.ReadOnly = true; // allow selection but no edits
                tb.BorderStyle = BorderStyle.None;
                tb.BackColor = System.Drawing.SystemColors.Window;
                // No need to capture or reset text – leaving it untouched avoids unwanted changes
            }
        }

        private void DataGridViewPalette_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (_bulkUpdating)
            {
                return;
            }

            // Custom draw for API column (#2)
            if (e.RowIndex >= 0 && e.ColumnIndex == 2)
            {
                // Draw background + border first to preserve gridlines
                e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);

                if (e.Value is string s && s.Length > 0)
                {
                    var font = e.CellStyle.Font ?? this.Font;
                    var fore = e.CellStyle.ForeColor;
                    int y = e.CellBounds.Y + 2;
                    foreach (var line in s.Split('\n'))
                    {
                        TextRenderer.DrawText(e.Graphics, line, font,
                            new System.Drawing.Rectangle(e.CellBounds.X + 2, y, e.CellBounds.Width - 4, font.Height),
                            fore, TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis);
                        y += font.Height + 2;
                    }
                }

                // Highlight current cell border in red
                if (this.dataGridViewPalette.CurrentCell != null && e.RowIndex == this.dataGridViewPalette.CurrentCell.RowIndex && e.ColumnIndex == this.dataGridViewPalette.CurrentCell.ColumnIndex)
                {
                    using (var pen = new System.Drawing.Pen(System.Drawing.Color.Red, 2))
                    {
                        var rect = e.CellBounds;
                        rect.Width -= 1;
                        rect.Height -= 1;
                        e.Graphics.DrawRectangle(pen, rect);
                    }
                }

                e.Handled = true;
                return;
            }

            if (this.dataGridViewPalette.CurrentCell != null && e.RowIndex == this.dataGridViewPalette.CurrentCell.RowIndex && e.ColumnIndex == this.dataGridViewPalette.CurrentCell.ColumnIndex)
            {
                // Let default paint
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                using (var pen = new System.Drawing.Pen(System.Drawing.Color.Red, 2))
                {
                    var rect = e.CellBounds;
                    rect.Width -= 1;
                    rect.Height -= 1;
                    e.Graphics.DrawRectangle(pen, rect);
                }

                e.Handled = true;
            }
        }

        private void DataGridViewPalette_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll && !_bulkUpdating)
            {
                dataGridViewPalette.Refresh();
            }
        }

        private static void TryCopy(string text)
        {
            const int retries = 5;
            const int delay = 100;
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    System.Windows.Forms.Clipboard.SetDataObject(text, true);
                    return;
                }
                catch (System.Runtime.InteropServices.ExternalException)
                {
                    System.Threading.Thread.Sleep(delay);
                }
            }
        }

        private static System.Type GetRibbonColorsOwner(System.Type t)
        {
            return t?.BaseType;
        }

        /// <summary>
        /// Extracts the colour array from a palette via <c>ColorTable.Colors</c> (or <c>Colours</c>).
        /// </summary>
        private static System.Drawing.Color[] TryGetPaletteColors(Krypton.Toolkit.PaletteBase palette)
        {
            if (palette == null)
            {
                return null;
            }

            try
            {
                var ct = palette.ColorTable;
                if (ct != null)
                {
                    var prop = ct.GetType().GetProperty("Colors", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                               ?? ct.GetType().GetProperty("Colours", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

                    if (prop != null && prop.PropertyType == typeof(System.Drawing.Color[]))
                    {
                        return prop.GetValue(ct) as System.Drawing.Color[];
                    }
                }
            }
            catch (System.IndexOutOfRangeException)
            {
                // Ignore faulty palettes; caller treats palette as sparse/incompatible.
            }

            return null;
        }

        private void MergeEnumMethodMapping(System.Collections.Generic.Dictionary<System.Reflection.MemberInfo, string> newMap)
        {
            if (_methodEnumMapping == null)
            {
                _methodEnumMapping = newMap;
                return;
            }

            foreach (var kv in newMap)
            {
                if (!_methodEnumMapping.ContainsKey(kv.Key))
                {
                    _methodEnumMapping[kv.Key] = kv.Value;
                }
            }
        }

        private void BtnBrowseSource_Click(object sender, System.EventArgs e)
        {
            using (var dlg = new Krypton.Toolkit.KryptonFolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(_sourcePath) && System.IO.Directory.Exists(_sourcePath))
                {
                    dlg.SelectedPath = _sourcePath;
                }

                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    SetSourcePath(dlg.SelectedPath);
                }
            }
        }

        private void SetSourcePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (string.Equals(_sourcePath, path, System.StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            _sourcePath = path;
            _isSourceValid = IsValidSourcePath(_sourcePath);
            if (this.textSourcePath != null)
            {
                this.textSourcePath.Text = path;
            }

            // persist immediately
            var ws = _winInfo ?? new WindowStateInfo();
            ws.SourcePath = path;
            ws.Left = this.Left;
            ws.Top = this.Top;
            ws.Width = this.Width;
            ws.Height = this.Height;
            ws.State = this.WindowState;
            ws.LastTheme = comboTheme.SelectedItem as string;
            _winStore.Save(ws);

            RecheckAllPalettes();

            UpdateUIState();
        }

        private void RecheckAllPalettes()
        {
            if (string.IsNullOrWhiteSpace(_sourcePath))
            {
                return;
            }

            for (int p = 0; p < _palettes.Count; p++)
            {
                var palette = _palettes[p];
                var col = this.dataGridViewPalette.Columns[palette.GetType().FullName];
                if (col == null)
                {
                    continue;
                }
                int colIndex = col.Index;

                var issues = Classes.ThemeArrayInspector.GetIssues(palette.GetType(), _sourcePath);
                if (issues == null || issues.IsClean)
                {
                    continue;
                }

                foreach (int idx in issues.MissingIndices)
                {
                    if (idx >= 0 && idx < this.dataGridViewPalette.Rows.Count)
                    {
                        var c = this.dataGridViewPalette.Rows[idx].Cells[colIndex];
                        if (string.IsNullOrEmpty(c.ErrorText)) c.ErrorText = "Missing value";
                        if (!string.IsNullOrEmpty(c.ErrorText) && (c.ToolTipText == null || !c.ToolTipText.Contains(c.ErrorText)))
                        {
                            c.ToolTipText += " : " + c.ErrorText;
                        }
                    }
                }
                foreach (int idx in issues.UnlabelledIndices)
                {
                    if (idx >= 0 && idx < this.dataGridViewPalette.Rows.Count)
                    {
                        var c = this.dataGridViewPalette.Rows[idx].Cells[colIndex];
                        if (string.IsNullOrEmpty(c.ErrorText)) c.ErrorText = "Unmarked value";
                        if (!string.IsNullOrEmpty(c.ErrorText) && (c.ToolTipText == null || !c.ToolTipText.Contains(c.ErrorText)))
                        {
                            c.ToolTipText += " : " + c.ErrorText;
                        }
                    }
                }
                foreach (int idx in issues.OutOfOrderIndices)
                {
                    if (idx >= 0 && idx < this.dataGridViewPalette.Rows.Count)
                    {
                        var c = this.dataGridViewPalette.Rows[idx].Cells[colIndex];
                        if (string.IsNullOrEmpty(c.ErrorText)) c.ErrorText = "Out-of-order";
                        if (!string.IsNullOrEmpty(c.ErrorText) && (c.ToolTipText == null || !c.ToolTipText.Contains(c.ErrorText)))
                        {
                            c.ToolTipText += " : " + c.ErrorText;
                        }
                    }
                }
                foreach (int idx in issues.ExtraIndices)
                {
                    if (idx >= 0 && idx < this.dataGridViewPalette.Rows.Count)
                    {
                        var c = this.dataGridViewPalette.Rows[idx].Cells[colIndex];
                        if (string.IsNullOrEmpty(c.ErrorText)) c.ErrorText = "Extra entry";
                        if (!string.IsNullOrEmpty(c.ErrorText) && (c.ToolTipText == null || !c.ToolTipText.Contains(c.ErrorText)))
                        {
                            c.ToolTipText += " : " + c.ErrorText;
                        }
                    }
                }
            }

            this.dataGridViewPalette.Refresh();
        }

        private void BtnClearSource_Click(object sender, System.EventArgs e)
        {
            ClearSourcePath();
        }

        private void ClearSourcePath()
        {
            _sourcePath = null;
            _isSourceValid = false;

            if (this.textSourcePath != null)
            {
                this.textSourcePath.Text = string.Empty;
            }

            // Persist cleared setting
            var ws = _winInfo ?? new WindowStateInfo();
            ws.SourcePath = null;
            ws.Left = this.Left;
            ws.Top = this.Top;
            ws.Width = this.Width;
            ws.Height = this.Height;
            ws.State = this.WindowState;
            ws.LastTheme = comboTheme.SelectedItem as string;
            _winStore.Save(ws);

            UpdateUIState();
        }
    }
}