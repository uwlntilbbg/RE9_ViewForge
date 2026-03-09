#nullable enable
using System;
using System.Collections.Generic;
using Hexa.NET.ImGui;
using REFrameworkNET.Callbacks;
using REFrameworkNET.Attributes;
using REFrameworkNET;
using REFrameworkNETPluginConfig;
using app;
using via;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Reflection;
using InteractLimitType = app.InteractManager.InteractLimitType;


namespace RE9_ViewForge
{
	public class ViewForgePlugin
	{
		#region PLUGIN_INFO

		/*PLUGIN INFO*/
		public const string PLUGIN_NAME = "RE9_ViewForge";
		public const string COPYRIGHT = "";
		public const string COMPANY = "Community Mod Build";

		public const string GUID = "RE9_ViewForge";
		public const string VERSION = "1.1.0";

		public const string GUID_AND_V_VERSION = GUID + " v" + VERSION;

		#endregion



		/* VARIABLES */
		//Const
		public const float DEFAULT_TPS_FOV = 40f;
		public const float DEFAULT_TPS_ADS_FOV = 25f;
		public const float DEFAULT_FPS_FOV = 46f;
		public const float DEFAULT_FPS_ADS_FOV = 40f;
		public const float DEFAULT_EASE_STRENGTH = 0.5f;
		public const float DEFAULT_FOV_SMOOTHING_FACTOR = 0.25f;

		public const float MIN_FOV = -180f;
		public const float MAX_FOV = 360f;
		public const float FOV_STEP = 0.1f;

		public const int MIN_PROFILE = 1;
		public const int MAX_PROFILE = 4;
		public const int PROFILE_VIEW_KEEP_CURRENT = 0;
		public const int PROFILE_VIEW_TPS = 1;
		public const int PROFILE_VIEW_FPS = 2;

		private static readonly string[] _profileNames = new string[]
		{
			"Balanced",
			"Action Push",
			"Wide Scout",
			"Director Shot"
		};

		private static readonly string[] _profileViewModeNames = new string[]
		{
			"Keep current",
			"Third person (TPS)",
			"First person (FPS)"
		};

		//Config
		private static Vector4 _colorRed = new Vector4(1f, 0.4f, 0.4f, 1f);
		private static Config _config = new Config(GUID);

		//General
		private static ConfigEntry<bool> _enabled = _config.Add("Enabled", true);
		private static ConfigEntry<int> _activeProfile = _config.Add("Active profile", 1);
		private static ConfigEntry<bool> _showDebugOverlay = _config.Add("Show debug overlay", false);
		private static ConfigEntry<bool> _autoProfileByContext = _config.Add("Auto profile by context", false);
		private static ConfigEntry<int> _profileForTPS = _config.Add("Profile for TPS", 1);
		private static ConfigEntry<int> _profileForFPS = _config.Add("Profile for FPS", 2);
		private static ConfigEntry<int> _profileForADS = _config.Add("Profile for ADS", 3);
		private static ConfigEntry<bool> _adsProfileHasPriority = _config.Add("ADS profile has priority", true);
		private static ConfigEntry<bool> _showMiniHud = _config.Add("Show mini HUD", true);
		private static ConfigEntry<bool> _enableHotkeys = _config.Add("Enable hotkeys", true);
		private static ConfigEntry<bool> _hotkeysBlockWhenTyping = _config.Add("Block hotkeys when typing", false);
		private static ConfigEntry<bool> _profileHotkeysForceManual = _config.Add("Profile hotkeys force manual mode", true);
		private static ConfigEntry<bool> _disableSpecialCamera = _config.Add("Disable special cameras", false);
		private static ConfigEntry<int> _hotkeyProfile1 = _config.Add("Hotkey profile P1", (int)ImGuiKey.F1);
		private static ConfigEntry<int> _hotkeyProfile2 = _config.Add("Hotkey profile P2", (int)ImGuiKey.F2);
		private static ConfigEntry<int> _hotkeyProfile3 = _config.Add("Hotkey profile P3", (int)ImGuiKey.F3);
		private static ConfigEntry<int> _hotkeyProfile4 = _config.Add("Hotkey profile P4", (int)ImGuiKey.F4);
		private static ConfigEntry<int> _hotkeyToggleAutoProfile = _config.Add("Hotkey toggle auto profile", (int)ImGuiKey.F6);
		private static ConfigEntry<int> _hotkeyHoldVanillaFOV = _config.Add("Hotkey hold vanilla FOV", (int)ImGuiKey.LeftAlt);
		private static ConfigEntry<bool> _hotkeyProfile1Ctrl = _config.Add("Hotkey profile P1 Ctrl", false);
		private static ConfigEntry<bool> _hotkeyProfile1Shift = _config.Add("Hotkey profile P1 Shift", false);
		private static ConfigEntry<bool> _hotkeyProfile1Alt = _config.Add("Hotkey profile P1 Alt", false);
		private static ConfigEntry<bool> _hotkeyProfile2Ctrl = _config.Add("Hotkey profile P2 Ctrl", false);
		private static ConfigEntry<bool> _hotkeyProfile2Shift = _config.Add("Hotkey profile P2 Shift", false);
		private static ConfigEntry<bool> _hotkeyProfile2Alt = _config.Add("Hotkey profile P2 Alt", false);
		private static ConfigEntry<bool> _hotkeyProfile3Ctrl = _config.Add("Hotkey profile P3 Ctrl", false);
		private static ConfigEntry<bool> _hotkeyProfile3Shift = _config.Add("Hotkey profile P3 Shift", false);
		private static ConfigEntry<bool> _hotkeyProfile3Alt = _config.Add("Hotkey profile P3 Alt", false);
		private static ConfigEntry<bool> _hotkeyProfile4Ctrl = _config.Add("Hotkey profile P4 Ctrl", false);
		private static ConfigEntry<bool> _hotkeyProfile4Shift = _config.Add("Hotkey profile P4 Shift", false);
		private static ConfigEntry<bool> _hotkeyProfile4Alt = _config.Add("Hotkey profile P4 Alt", false);
		private static ConfigEntry<bool> _hotkeyToggleAutoCtrl = _config.Add("Hotkey toggle auto profile Ctrl", false);
		private static ConfigEntry<bool> _hotkeyToggleAutoShift = _config.Add("Hotkey toggle auto profile Shift", false);
		private static ConfigEntry<bool> _hotkeyToggleAutoAlt = _config.Add("Hotkey toggle auto profile Alt", false);
		private static ConfigEntry<bool> _hotkeyHoldVanillaCtrl = _config.Add("Hotkey hold vanilla FOV Ctrl", false);
		private static ConfigEntry<bool> _hotkeyHoldVanillaShift = _config.Add("Hotkey hold vanilla FOV Shift", false);
		private static ConfigEntry<bool> _hotkeyHoldVanillaAlt = _config.Add("Hotkey hold vanilla FOV Alt", false);
		private static ConfigEntry<bool> _applyViewModeFromProfile = _config.Add("Apply view mode from profile", false);
		private static ConfigEntry<int> _profileViewModeP1 = _config.Add("P1 view mode", PROFILE_VIEW_KEEP_CURRENT);
		private static ConfigEntry<int> _profileViewModeP2 = _config.Add("P2 view mode", PROFILE_VIEW_KEEP_CURRENT);
		private static ConfigEntry<int> _profileViewModeP3 = _config.Add("P3 view mode", PROFILE_VIEW_KEEP_CURRENT);
		private static ConfigEntry<int> _profileViewModeP4 = _config.Add("P4 view mode", PROFILE_VIEW_KEEP_CURRENT);

		//TPS profile 1 (legacy keys kept for backward compatibility)
		private static ConfigEntry<float> _tpsFov = _config.Add("TPS FOV", DEFAULT_TPS_FOV);
		private static ConfigEntry<float> _tpsFovADS = _config.Add("TPS ADS FOV", DEFAULT_TPS_ADS_FOV);
		private static ConfigEntry<bool> _tpsFixedADSFOV = _config.Add("TPS Fixed ADS FOV", false);
		private static ConfigEntry<bool> _tpsDisableADSZoom = _config.Add("TPS Disable ADS zoom / FOV change", false);
		private static ConfigEntry<bool> _tpsForceExactLookFOV = _config.Add("TPS Force exact look FOV", false);
		private static ConfigEntry<bool> _tpsForceExactADSFOV = _config.Add("TPS Force exact ADS FOV", false);
		private static ConfigEntry<bool> _tpsForceExactFOV = _config.Add("TPS Force exact FOV", false);

		//TPS profile 2-4
		private static ConfigEntry<float> _tpsFovProfile2 = _config.Add("P2 TPS FOV", 48f);
		private static ConfigEntry<float> _tpsFovADSProfile2 = _config.Add("P2 TPS ADS FOV", 30f);
		private static ConfigEntry<float> _tpsFovProfile3 = _config.Add("P3 TPS FOV", 56f);
		private static ConfigEntry<float> _tpsFovADSProfile3 = _config.Add("P3 TPS ADS FOV", 36f);
		private static ConfigEntry<float> _tpsFovProfile4 = _config.Add("P4 TPS FOV", 64f);
		private static ConfigEntry<float> _tpsFovADSProfile4 = _config.Add("P4 TPS ADS FOV", 40f);

		//FPS profile 1 (legacy keys kept for backward compatibility)
		private static ConfigEntry<float> _fpsFOV = _config.Add("FPS FOV", DEFAULT_FPS_FOV);
		private static ConfigEntry<float> _fpsFOVADS = _config.Add("FPS ADS FOV", DEFAULT_FPS_ADS_FOV);
		private static ConfigEntry<bool> _fpsFixedADSFOV = _config.Add("FPS Fixed ADS FOV", false);
		private static ConfigEntry<bool> _fpsDisableADSZoom = _config.Add("FPS Disable ADS zoom / FOV change", false);
		private static ConfigEntry<bool> _fpsForceExactLookFOV = _config.Add("FPS Force exact look FOV", false);
		private static ConfigEntry<bool> _fpsForceExactADSFOV = _config.Add("FPS Force exact ADS FOV", false);
		private static ConfigEntry<bool> _fpsForceExactFOV = _config.Add("FPS Force exact FOV", false);

		//FPS profile 2-4
		private static ConfigEntry<float> _fpsFovProfile2 = _config.Add("P2 FPS FOV", 50f);
		private static ConfigEntry<float> _fpsFovADSProfile2 = _config.Add("P2 FPS ADS FOV", 42f);
		private static ConfigEntry<float> _fpsFovProfile3 = _config.Add("P3 FPS FOV", 58f);
		private static ConfigEntry<float> _fpsFovADSProfile3 = _config.Add("P3 FPS ADS FOV", 48f);
		private static ConfigEntry<float> _fpsFovProfile4 = _config.Add("P4 FPS FOV", 66f);
		private static ConfigEntry<float> _fpsFovADSProfile4 = _config.Add("P4 FPS ADS FOV", 54f);

		//Modifiers
		private static ConfigEntry<float> _easeStrength = _config.Add("Ease strength", DEFAULT_EASE_STRENGTH);
		private static ConfigEntry<bool> _enableFOVSmoothing = _config.Add("Enable FOV smoothing", true);
		private static ConfigEntry<float> _fovSmoothingFactor = _config.Add("FOV smoothing factor", DEFAULT_FOV_SMOOTHING_FACTOR);
		private static ConfigEntry<bool> _failsafeEnabled = _config.Add("Failsafe: enable low-FPS fallback", false);
		private static ConfigEntry<float> _failsafeFpsThreshold = _config.Add("Failsafe: FPS threshold", 45f);
		private static ConfigEntry<float> _failsafeTPSFOV = _config.Add("Failsafe: TPS FOV", 60f);
		private static ConfigEntry<float> _failsafeFPSFOV = _config.Add("Failsafe: FPS FOV", 70f);

		//Singletons
		private static InteractManager? _interactManager;
		private static CharacterManager? _characterManager;

		//Variables
		private static float? _smoothedFOV;
		private static int _lastSmoothingFrame = -1;
		private static bool _failsafeActive;
		private static float _lastFramerate;

		private static float _lastRawFOV;
		private static float _lastTargetFOV;
		private static float _lastAppliedFOV;
		private static bool _lastIsADS;
		private static PlayerMode _lastPlayerMode = PlayerMode.TPS;
		private static int _lastResolvedProfile = 1;
		private static string _lastProfileSource = "manual";
		private static bool _isHoldingVanillaFOV;
		private static string _lastHotkeyEvent = "none";

		private const int HOTKEY_BIND_PROFILE_1 = 1;
		private const int HOTKEY_BIND_PROFILE_2 = 2;
		private const int HOTKEY_BIND_PROFILE_3 = 3;
		private const int HOTKEY_BIND_PROFILE_4 = 4;
		private const int HOTKEY_BIND_TOGGLE_AUTO = 5;
		private const int HOTKEY_BIND_HOLD_VANILLA = 6;

		private static readonly ImGuiKey[] _allImGuiKeys = Enum.GetValues<ImGuiKey>();
		private static int _pendingHotkeyBinding;
		private static bool _hotkeyBindWaitForRelease;
		private static int _hotkeyBindCaptureAfterFrame = -1;
		private static readonly Dictionary<int, string> _hotkeyInputTexts = new Dictionary<int, string>();

		[DllImport("user32.dll")]
		private static extern short GetAsyncKeyState(int vKey);

		private static bool _wasProfile1Down;
		private static bool _wasProfile2Down;
		private static bool _wasProfile3Down;
		private static bool _wasProfile4Down;
		private static bool _wasToggleAutoDown;

		private static float _resetTextSize = 0f;
		public static float ResetTextSize
		{
			get
			{
				if (_resetTextSize != 0f) return _resetTextSize;
				else return _resetTextSize = ImGui.CalcTextSize("Reset").X;
			}
		}



		/* METHODS */
		private static bool IsADS()
		{
			return _interactManager != null && (_interactManager.LimitType == InteractLimitType.Stance || _interactManager.LimitType == InteractLimitType.ScopeStance);
		}

		private static int GetActiveProfileNumber()
		{
			return Math.Clamp(_activeProfile.Value, MIN_PROFILE, MAX_PROFILE);
		}

		private static int GetClampedProfileNumber(int profileNumber)
		{
			return Math.Clamp(profileNumber, MIN_PROFILE, MAX_PROFILE);
		}

		private static string GetProfileName(int profileNumber)
		{
			int index = Math.Clamp(profileNumber, MIN_PROFILE, MAX_PROFILE) - 1;
			return _profileNames[index];
		}

		private static ConfigEntry<int> GetProfileViewModeEntry(int profileNumber)
		{
			return profileNumber switch
			{
				2 => _profileViewModeP2,
				3 => _profileViewModeP3,
				4 => _profileViewModeP4,
				_ => _profileViewModeP1,
			};
		}

		private static ConfigEntry<float> GetTPSFovEntry(int profileNumber)
		{
			return profileNumber switch
			{
				2 => _tpsFovProfile2,
				3 => _tpsFovProfile3,
				4 => _tpsFovProfile4,
				_ => _tpsFov,
			};
		}

		private static ConfigEntry<float> GetTPSADSFovEntry(int profileNumber)
		{
			return profileNumber switch
			{
				2 => _tpsFovADSProfile2,
				3 => _tpsFovADSProfile3,
				4 => _tpsFovADSProfile4,
				_ => _tpsFovADS,
			};
		}

		private static ConfigEntry<float> GetFPSFovEntry(int profileNumber)
		{
			return profileNumber switch
			{
				2 => _fpsFovProfile2,
				3 => _fpsFovProfile3,
				4 => _fpsFovProfile4,
				_ => _fpsFOV,
			};
		}

		private static ConfigEntry<float> GetFPSADSFovEntry(int profileNumber)
		{
			return profileNumber switch
			{
				2 => _fpsFovADSProfile2,
				3 => _fpsFovADSProfile3,
				4 => _fpsFovADSProfile4,
				_ => _fpsFOVADS,
			};
		}

		private static void NormalizeSettings()
		{
			_activeProfile.SetSilent(Math.Clamp(_activeProfile.Value, MIN_PROFILE, MAX_PROFILE));
			_profileForTPS.SetSilent(GetClampedProfileNumber(_profileForTPS.Value));
			_profileForFPS.SetSilent(GetClampedProfileNumber(_profileForFPS.Value));
			_profileForADS.SetSilent(GetClampedProfileNumber(_profileForADS.Value));
			_profileViewModeP1.SetSilent(Math.Clamp(_profileViewModeP1.Value, PROFILE_VIEW_KEEP_CURRENT, PROFILE_VIEW_FPS));
			_profileViewModeP2.SetSilent(Math.Clamp(_profileViewModeP2.Value, PROFILE_VIEW_KEEP_CURRENT, PROFILE_VIEW_FPS));
			_profileViewModeP3.SetSilent(Math.Clamp(_profileViewModeP3.Value, PROFILE_VIEW_KEEP_CURRENT, PROFILE_VIEW_FPS));
			_profileViewModeP4.SetSilent(Math.Clamp(_profileViewModeP4.Value, PROFILE_VIEW_KEEP_CURRENT, PROFILE_VIEW_FPS));
			_fovSmoothingFactor.SetSilent(Math.Clamp(_fovSmoothingFactor.Value, 0f, 1f));
			NormalizeHotkey(_hotkeyProfile1, ImGuiKey.F1);
			NormalizeHotkey(_hotkeyProfile2, ImGuiKey.F2);
			NormalizeHotkey(_hotkeyProfile3, ImGuiKey.F3);
			NormalizeHotkey(_hotkeyProfile4, ImGuiKey.F4);
			NormalizeHotkey(_hotkeyToggleAutoProfile, ImGuiKey.F6);
			NormalizeHotkey(_hotkeyHoldVanillaFOV, ImGuiKey.LeftAlt);
		}

		private static int GetResolvedProfileNumber(PlayerMode currentViewMode, bool isADS, out string source)
		{
			if (_autoProfileByContext.Value == false)
			{
				source = "manual";
				return GetActiveProfileNumber();
			}

			if (_adsProfileHasPriority.Value && isADS)
			{
				source = "auto-ads";
				return GetClampedProfileNumber(_profileForADS.Value);
			}

			if (currentViewMode == PlayerMode.FPS)
			{
				source = "auto-fps";
				return GetClampedProfileNumber(_profileForFPS.Value);
			}

			source = "auto-tps";
			return GetClampedProfileNumber(_profileForTPS.Value);
		}

		private static bool IsTypingInUI()
		{
			try
			{
				var io = ImGui.GetIO();
				return io.WantTextInput;
			}
			catch
			{
				return false;
			}
		}

		private static ImGuiKey GetConfiguredHotkey(ConfigEntry<int> hotkeyEntry)
		{
			int value = hotkeyEntry.Value;
			if (Enum.IsDefined(typeof(ImGuiKey), value) == false) return ImGuiKey.None;
			return (ImGuiKey)value;
		}

		private static void NormalizeHotkey(ConfigEntry<int> hotkeyEntry, ImGuiKey fallback)
		{
			int value = hotkeyEntry.Value;
			if (Enum.IsDefined(typeof(ImGuiKey), value)) return;
			hotkeyEntry.SetSilent((int)fallback);
		}

		private static string GetHotkeyDisplayName(ConfigEntry<int> hotkeyEntry, ConfigEntry<bool> hotkeyCtrlEntry, ConfigEntry<bool> hotkeyShiftEntry, ConfigEntry<bool> hotkeyAltEntry)
		{
			ImGuiKey key = GetConfiguredHotkey(hotkeyEntry);
			bool needCtrl = hotkeyCtrlEntry.Value;
			bool needShift = hotkeyShiftEntry.Value;
			bool needAlt = hotkeyAltEntry.Value;

			if (key == ImGuiKey.None)
			{
				return needCtrl || needShift || needAlt ? "Invalid (choose main key)" : "Disabled";
			}

			string combo = string.Empty;
			if (needCtrl) combo += "Ctrl+";
			if (needShift) combo += "Shift+";
			if (needAlt) combo += "Alt+";
			combo += key.ToString();
			return combo;
		}

		private static string GetHotkeyInputText(int bindingId, ConfigEntry<int> hotkeyEntry, ConfigEntry<bool> hotkeyCtrlEntry, ConfigEntry<bool> hotkeyShiftEntry, ConfigEntry<bool> hotkeyAltEntry)
		{
			if (_hotkeyInputTexts.TryGetValue(bindingId, out string? inputText) == false)
			{
				inputText = GetHotkeyDisplayName(hotkeyEntry, hotkeyCtrlEntry, hotkeyShiftEntry, hotkeyAltEntry);
				_hotkeyInputTexts[bindingId] = inputText;
			}

			return inputText;
		}

		private static void RefreshHotkeyInputText(int bindingId, ConfigEntry<int> hotkeyEntry, ConfigEntry<bool> hotkeyCtrlEntry, ConfigEntry<bool> hotkeyShiftEntry, ConfigEntry<bool> hotkeyAltEntry)
		{
			_hotkeyInputTexts[bindingId] = GetHotkeyDisplayName(hotkeyEntry, hotkeyCtrlEntry, hotkeyShiftEntry, hotkeyAltEntry);
		}

		private static bool TryParseHotkeyKeyToken(string token, out ImGuiKey key)
		{
			string normalized = token.Trim().ToUpperInvariant();
			key = ImGuiKey.None;
			if (normalized.Length == 1)
			{
				char c = normalized[0];
				if (c >= 'A' && c <= 'Z')
				{
					if (Enum.TryParse(c.ToString(), out ImGuiKey parsedLetterKey))
					{
						key = parsedLetterKey;
						return true;
					}
					return false;
				}
				if (c >= '0' && c <= '9')
				{
					if (Enum.TryParse("Key" + c, out ImGuiKey parsedDigitKey))
					{
						key = parsedDigitKey;
						return true;
					}
					return false;
				}
			}

			switch (normalized)
			{
				case "TAB": key = ImGuiKey.Tab; return true;
				case "SPACE": key = ImGuiKey.Space; return true;
				case "ENTER": key = ImGuiKey.Enter; return true;
				case "ESC":
				case "ESCAPE": key = ImGuiKey.Escape; return true;
				case "INSERT": key = ImGuiKey.Insert; return true;
				case "DELETE":
				case "DEL": key = ImGuiKey.Delete; return true;
				case "HOME": key = ImGuiKey.Home; return true;
				case "END": key = ImGuiKey.End; return true;
				case "PAGEUP":
				case "PGUP": key = ImGuiKey.PageUp; return true;
				case "PAGEDOWN":
				case "PGDN": key = ImGuiKey.PageDown; return true;
				case "UP":
				case "UPARROW": key = ImGuiKey.UpArrow; return true;
				case "DOWN":
				case "DOWNARROW": key = ImGuiKey.DownArrow; return true;
				case "LEFT":
				case "LEFTARROW": key = ImGuiKey.LeftArrow; return true;
				case "RIGHT":
				case "RIGHTARROW": key = ImGuiKey.RightArrow; return true;
			}

			if (normalized.StartsWith("F", StringComparison.Ordinal) && int.TryParse(normalized.Substring(1), out int fNumber))
			{
				if (fNumber >= 1 && fNumber <= 24)
				{
					if (Enum.TryParse("F" + fNumber, out ImGuiKey parsedFunctionKey))
					{
						key = parsedFunctionKey;
						return true;
					}
				}
			}

			return false;
		}

		private static bool TryParseHotkeyCombo(string inputText, out ImGuiKey key, out bool needCtrl, out bool needShift, out bool needAlt)
		{
			key = ImGuiKey.None;
			needCtrl = false;
			needShift = false;
			needAlt = false;

			if (string.IsNullOrWhiteSpace(inputText)) return false;

			string[] tokens = inputText.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			if (tokens.Length == 0) return false;

			for (int i = 0; i < tokens.Length; i++)
			{
				string token = tokens[i].Trim();
				if (token.Length == 0) continue;

				string upper = token.ToUpperInvariant();
				if (upper == "CTRL" || upper == "CONTROL")
				{
					needCtrl = true;
					continue;
				}
				if (upper == "SHIFT")
				{
					needShift = true;
					continue;
				}
				if (upper == "ALT")
				{
					needAlt = true;
					continue;
				}

				if (TryParseHotkeyKeyToken(token, out ImGuiKey parsedKey))
				{
					key = parsedKey;
					continue;
				}

				return false;
			}

			return key != ImGuiKey.None;
		}

		private static string GetProfileViewModeDisplayName(int value)
		{
			int clampedValue = Math.Clamp(value, PROFILE_VIEW_KEEP_CURRENT, PROFILE_VIEW_FPS);
			return _profileViewModeNames[clampedValue];
		}

		private static bool IsModifierKey(ImGuiKey key)
		{
			return key == ImGuiKey.LeftCtrl || key == ImGuiKey.RightCtrl ||
				key == ImGuiKey.LeftShift || key == ImGuiKey.RightShift ||
				key == ImGuiKey.LeftAlt || key == ImGuiKey.RightAlt ||
				key == ImGuiKey.LeftSuper || key == ImGuiKey.RightSuper;
		}

		private static bool IsNonBindableKeyName(string keyName)
		{
			return keyName.StartsWith("Mouse", StringComparison.Ordinal) ||
				keyName.StartsWith("Mod", StringComparison.Ordinal) ||
				keyName.StartsWith("Gamepad", StringComparison.Ordinal);
		}

		private static bool IsCtrlDown()
		{
			return IsHotkeyDown(ImGuiKey.LeftCtrl) || IsHotkeyDown(ImGuiKey.RightCtrl);
		}

		private static bool IsShiftDown()
		{
			return IsHotkeyDown(ImGuiKey.LeftShift) || IsHotkeyDown(ImGuiKey.RightShift);
		}

		private static bool IsAltDown()
		{
			return IsHotkeyDown(ImGuiKey.LeftAlt) || IsHotkeyDown(ImGuiKey.RightAlt);
		}

		private static bool AreHotkeyModifiersMatching(bool needCtrl, bool needShift, bool needAlt)
		{
			bool ctrlDown = IsCtrlDown();
			bool shiftDown = IsShiftDown();
			bool altDown = IsAltDown();

			// Exact match avoids accidental trigger of plain keys while a modifier is held.
			return ctrlDown == needCtrl && shiftDown == needShift && altDown == needAlt;
		}

		private static bool IsAnyNonModifierKeyDown()
		{
			for (int i = 0; i < _allImGuiKeys.Length; i++)
			{
				ImGuiKey key = _allImGuiKeys[i];
				if (key == ImGuiKey.None) continue;
				if (IsModifierKey(key)) continue;

				string keyName = key.ToString();
				if (IsNonBindableKeyName(keyName)) continue;
				if (IsHotkeyDown(key)) return true;
			}

			return false;
		}

		private static bool IsAnyBindingInputDown()
		{
			return IsCtrlDown() || IsShiftDown() || IsAltDown() || IsAnyNonModifierKeyDown();
		}

		private static bool TryGetVirtualKeyCode(ImGuiKey key, out int virtualKeyCode)
		{
			string keyName = key.ToString();
			switch (keyName)
			{
				case "LeftCtrl": virtualKeyCode = 0xA2; return true;
				case "RightCtrl": virtualKeyCode = 0xA3; return true;
				case "LeftShift": virtualKeyCode = 0xA0; return true;
				case "RightShift": virtualKeyCode = 0xA1; return true;
				case "LeftAlt": virtualKeyCode = 0xA4; return true;
				case "RightAlt": virtualKeyCode = 0xA5; return true;
				case "Tab": virtualKeyCode = 0x09; return true;
				case "Space": virtualKeyCode = 0x20; return true;
				case "Enter": virtualKeyCode = 0x0D; return true;
				case "Escape": virtualKeyCode = 0x1B; return true;
				case "Insert": virtualKeyCode = 0x2D; return true;
				case "Delete": virtualKeyCode = 0x2E; return true;
				case "Home": virtualKeyCode = 0x24; return true;
				case "End": virtualKeyCode = 0x23; return true;
				case "PageUp": virtualKeyCode = 0x21; return true;
				case "PageDown": virtualKeyCode = 0x22; return true;
				case "UpArrow": virtualKeyCode = 0x26; return true;
				case "DownArrow": virtualKeyCode = 0x28; return true;
				case "LeftArrow": virtualKeyCode = 0x25; return true;
				case "RightArrow": virtualKeyCode = 0x27; return true;
			}

			if (keyName.Length == 1)
			{
				char c = keyName[0];
				if (c >= 'A' && c <= 'Z')
				{
					virtualKeyCode = c;
					return true;
				}
				if (c >= '0' && c <= '9')
				{
					virtualKeyCode = c;
					return true;
				}
			}

			if (keyName.StartsWith("F", StringComparison.Ordinal) && int.TryParse(keyName.Substring(1), out int functionKeyNumber))
			{
				if (functionKeyNumber >= 1 && functionKeyNumber <= 24)
				{
					virtualKeyCode = 0x6F + functionKeyNumber;
					return true;
				}
			}

			virtualKeyCode = 0;
			return false;
		}

		private static bool IsHotkeyDown(ImGuiKey key)
		{
			if (ImGui.IsKeyDown(key)) return true;

			if (TryGetVirtualKeyCode(key, out int virtualKeyCode))
			{
				return (GetAsyncKeyState(virtualKeyCode) & 0x8000) != 0;
			}

			return false;
		}

		private static bool IsHotkeyPressed(ImGuiKey key, bool needCtrl, bool needShift, bool needAlt, ref bool wasDown)
		{
			if (key == ImGuiKey.None)
			{
				wasDown = false;
				return false;
			}

			bool modifiersMatch = AreHotkeyModifiersMatching(needCtrl, needShift, needAlt);
			bool isDown = IsHotkeyDown(key);
			bool pressedEdge = modifiersMatch && isDown && wasDown == false;

			bool pressedAsync = false;
			if (modifiersMatch && TryGetVirtualKeyCode(key, out int virtualKeyCode))
			{
				pressedAsync = (GetAsyncKeyState(virtualKeyCode) & 0x0001) != 0;
			}

			bool pressed = pressedEdge || pressedAsync;
			wasDown = isDown;
			return pressed;
		}

		private static bool TryCaptureHotkeyCombo(out ImGuiKey capturedKey, out bool needCtrl, out bool needShift, out bool needAlt)
		{
			needCtrl = IsCtrlDown();
			needShift = IsShiftDown();
			needAlt = IsAltDown();

			for (int i = 0; i < _allImGuiKeys.Length; i++)
			{
				ImGuiKey key = _allImGuiKeys[i];
				if (key == ImGuiKey.None) continue;
				if (IsModifierKey(key)) continue;

				string keyName = key.ToString();
				if (IsNonBindableKeyName(keyName)) continue;

				bool pressed = ImGui.IsKeyPressed(key, false);
				if (pressed == false && TryGetVirtualKeyCode(key, out int virtualKeyCode))
				{
					pressed = (GetAsyncKeyState(virtualKeyCode) & 0x0001) != 0;
				}
				// Fallback for cases where the key was already held when capture became active.
				if (pressed == false)
				{
					pressed = IsHotkeyDown(key);
				}

				if (pressed)
				{
					capturedKey = key;
					return true;
				}
			}

			capturedKey = ImGuiKey.None;
			return false;
		}

		private static void DrawHotkeyBindingRow(string actionLabel, string description, ConfigEntry<int> hotkeyEntry, ConfigEntry<bool> hotkeyCtrlEntry, ConfigEntry<bool> hotkeyShiftEntry, ConfigEntry<bool> hotkeyAltEntry, ImGuiKey defaultKey, bool defaultCtrl, bool defaultShift, bool defaultAlt, int bindingId)
		{
			ImGui.Text(actionLabel + ": " + GetHotkeyDisplayName(hotkeyEntry, hotkeyCtrlEntry, hotkeyShiftEntry, hotkeyAltEntry));
			DrawHelpMarker(description);
			string hotkeyInput = GetHotkeyInputText(bindingId, hotkeyEntry, hotkeyCtrlEntry, hotkeyShiftEntry, hotkeyAltEntry);
			if (ImGui.InputText("Combo##HotkeyInput" + bindingId, ref hotkeyInput, (nuint)48))
			{
				_hotkeyInputTexts[bindingId] = hotkeyInput;
			}
			ImGui.SameLine();
			if (ImGui.Button("Apply##HotkeyApply" + bindingId))
			{
				if (TryParseHotkeyCombo(hotkeyInput, out ImGuiKey parsedKey, out bool needCtrl, out bool needShift, out bool needAlt))
				{
					hotkeyEntry.Set((int)parsedKey);
					hotkeyCtrlEntry.Set(needCtrl);
					hotkeyShiftEntry.Set(needShift);
					hotkeyAltEntry.Set(needAlt);
					_pendingHotkeyBinding = 0;
					_hotkeyBindWaitForRelease = false;
					RefreshHotkeyInputText(bindingId, hotkeyEntry, hotkeyCtrlEntry, hotkeyShiftEntry, hotkeyAltEntry);
					_lastHotkeyEvent = actionLabel + " bind updated";
				}
				else
				{
					_lastHotkeyEvent = "Invalid hotkey text for " + actionLabel;
				}
			}

			bool waitingForBind = _pendingHotkeyBinding == bindingId;
			if (waitingForBind == false && ImGui.Button("Set##HotkeySet" + bindingId))
			{
				_pendingHotkeyBinding = bindingId;
				_hotkeyBindWaitForRelease = true;
				_hotkeyBindCaptureAfterFrame = ImGui.GetFrameCount() + 2;
				_lastHotkeyEvent = actionLabel + " waiting for combo";
			}
			else if (waitingForBind)
			{
				ImGui.TextColored(_colorRed, "Capturing...");
			}
			ImGui.SameLine();
			if (ImGui.Button("Clear##HotkeyClear" + bindingId))
			{
				hotkeyEntry.Set((int)ImGuiKey.None);
				hotkeyCtrlEntry.Set(false);
				hotkeyShiftEntry.Set(false);
				hotkeyAltEntry.Set(false);
				RefreshHotkeyInputText(bindingId, hotkeyEntry, hotkeyCtrlEntry, hotkeyShiftEntry, hotkeyAltEntry);
				if (_pendingHotkeyBinding == bindingId)
				{
					_pendingHotkeyBinding = 0;
					_hotkeyBindWaitForRelease = false;
					_hotkeyBindCaptureAfterFrame = -1;
				}
			}
			ImGui.SameLine();
			if (ImGui.Button("Default##HotkeyDefault" + bindingId))
			{
				hotkeyEntry.Set((int)defaultKey);
				hotkeyCtrlEntry.Set(defaultCtrl);
				hotkeyShiftEntry.Set(defaultShift);
				hotkeyAltEntry.Set(defaultAlt);
				RefreshHotkeyInputText(bindingId, hotkeyEntry, hotkeyCtrlEntry, hotkeyShiftEntry, hotkeyAltEntry);
				if (_pendingHotkeyBinding == bindingId)
				{
					_pendingHotkeyBinding = 0;
					_hotkeyBindWaitForRelease = false;
					_hotkeyBindCaptureAfterFrame = -1;
				}
			}
			if (waitingForBind)
			{
				ImGui.SameLine();
				if (ImGui.Button("Cancel##HotkeyCancel" + bindingId))
				{
					_pendingHotkeyBinding = 0;
					_hotkeyBindWaitForRelease = false;
					_hotkeyBindCaptureAfterFrame = -1;
					_lastHotkeyEvent = actionLabel + " bind capture canceled";
				}
			}
			waitingForBind = _pendingHotkeyBinding == bindingId;

			if (waitingForBind)
			{
				int frameId = ImGui.GetFrameCount();
				if (_hotkeyBindCaptureAfterFrame != -1 && frameId < _hotkeyBindCaptureAfterFrame)
				{
					ImGui.TextColored(_colorRed, "Waiting for input...");
				}
				else if (_hotkeyBindWaitForRelease)
				{
					if (TryCaptureHotkeyCombo(out ImGuiKey capturedKey, out bool needCtrl, out bool needShift, out bool needAlt))
					{
						hotkeyEntry.Set((int)capturedKey);
						hotkeyCtrlEntry.Set(needCtrl);
						hotkeyShiftEntry.Set(needShift);
						hotkeyAltEntry.Set(needAlt);
						RefreshHotkeyInputText(bindingId, hotkeyEntry, hotkeyCtrlEntry, hotkeyShiftEntry, hotkeyAltEntry);
						_pendingHotkeyBinding = 0;
						_hotkeyBindWaitForRelease = false;
						_hotkeyBindCaptureAfterFrame = -1;
						_lastHotkeyEvent = actionLabel + " bind updated (capture)";
					}
					else
					if (IsAnyBindingInputDown())
					{
						ImGui.TextColored(_colorRed, "Release held keys, then press combo...");
					}
					else
					{
						_hotkeyBindWaitForRelease = false;
					}
				}
				else
				{
					ImGui.TextColored(_colorRed, "Waiting for combo (example: Ctrl+Q)...");
					if (TryCaptureHotkeyCombo(out ImGuiKey capturedKey, out bool needCtrl, out bool needShift, out bool needAlt))
					{
						hotkeyEntry.Set((int)capturedKey);
						hotkeyCtrlEntry.Set(needCtrl);
						hotkeyShiftEntry.Set(needShift);
						hotkeyAltEntry.Set(needAlt);
						RefreshHotkeyInputText(bindingId, hotkeyEntry, hotkeyCtrlEntry, hotkeyShiftEntry, hotkeyAltEntry);
						_pendingHotkeyBinding = 0;
						_hotkeyBindWaitForRelease = false;
						_hotkeyBindCaptureAfterFrame = -1;
						_lastHotkeyEvent = actionLabel + " bind updated (capture)";
					}
				}
			}

			ImGui.Spacing();
		}

		private static void DrawProfileViewModeRow(int profileNumber)
		{
			ConfigEntry<int> viewModeEntry = GetProfileViewModeEntry(profileNumber);
			int currentValue = Math.Clamp(viewModeEntry.Value, PROFILE_VIEW_KEEP_CURRENT, PROFILE_VIEW_FPS);

			ImGui.Text("P" + profileNumber + " view mode: " + GetProfileViewModeDisplayName(currentValue));
			DrawHelpMarker("Choose which camera mode this profile should force: keep current / TPS / FPS.");
			ImGui.SameLine();
			if (ImGui.Button("Change##ProfileViewMode" + profileNumber))
			{
				int nextValue = currentValue + 1;
				if (nextValue > PROFILE_VIEW_FPS) nextValue = PROFILE_VIEW_KEEP_CURRENT;
				viewModeEntry.Set(nextValue);
			}
			ImGui.SameLine();
			if (ImGui.Button("Default##ProfileViewModeDefault" + profileNumber))
			{
				viewModeEntry.Set(PROFILE_VIEW_KEEP_CURRENT);
			}
		}

		private static void ApplyProfileViewModeIfNeeded(int profileNumber, ref PlayerMode currentViewMode)
		{
			if (_applyViewModeFromProfile.Value == false) return;
			if (_autoProfileByContext.Value) return;
			if (_characterManager == null || _characterManager.PlayerContextFast == null) return;

			int selectedViewMode = Math.Clamp(GetProfileViewModeEntry(profileNumber).Value, PROFILE_VIEW_KEEP_CURRENT, PROFILE_VIEW_FPS);
			if (selectedViewMode == PROFILE_VIEW_KEEP_CURRENT) return;

			PlayerMode targetMode = selectedViewMode == PROFILE_VIEW_FPS ? PlayerMode.FPS : PlayerMode.TPS;
			if (currentViewMode == targetMode) return;

			try
			{
				_characterManager.PlayerContextFast.CurrentViewMode = targetMode;
				currentViewMode = targetMode;
			}
			catch { }
		}

		private static void ApplyProfileHotkey(int profile)
		{
			_activeProfile.Set(profile);
			_lastHotkeyEvent = "Profile -> P" + profile;
			if (_profileHotkeysForceManual.Value && _autoProfileByContext.Value)
			{
				_autoProfileByContext.Set(false);
				_lastHotkeyEvent += " (auto off)";
			}
		}

		private static void ProcessHotkeys()
		{
			_isHoldingVanillaFOV = false;
			if (_pendingHotkeyBinding != 0)
			{
				if (IsHotkeyDown(ImGuiKey.Escape))
				{
					_pendingHotkeyBinding = 0;
					_hotkeyBindWaitForRelease = false;
				}
				ResetHotkeyEdgeState();
				return;
			}
			if (_enableHotkeys.Value == false)
			{
				ResetHotkeyEdgeState();
				return;
			}
			if (_hotkeysBlockWhenTyping.Value && IsTypingInUI())
			{
				ResetHotkeyEdgeState();
				return;
			}

			ImGuiKey hotkeyProfile1 = GetConfiguredHotkey(_hotkeyProfile1);
			ImGuiKey hotkeyProfile2 = GetConfiguredHotkey(_hotkeyProfile2);
			ImGuiKey hotkeyProfile3 = GetConfiguredHotkey(_hotkeyProfile3);
			ImGuiKey hotkeyProfile4 = GetConfiguredHotkey(_hotkeyProfile4);
			ImGuiKey hotkeyToggleAuto = GetConfiguredHotkey(_hotkeyToggleAutoProfile);
			ImGuiKey hotkeyHoldVanilla = GetConfiguredHotkey(_hotkeyHoldVanillaFOV);

			_isHoldingVanillaFOV = hotkeyHoldVanilla != ImGuiKey.None &&
				AreHotkeyModifiersMatching(_hotkeyHoldVanillaCtrl.Value, _hotkeyHoldVanillaShift.Value, _hotkeyHoldVanillaAlt.Value) &&
				IsHotkeyDown(hotkeyHoldVanilla);
			if (IsHotkeyPressed(hotkeyProfile1, _hotkeyProfile1Ctrl.Value, _hotkeyProfile1Shift.Value, _hotkeyProfile1Alt.Value, ref _wasProfile1Down)) ApplyProfileHotkey(1);
			if (IsHotkeyPressed(hotkeyProfile2, _hotkeyProfile2Ctrl.Value, _hotkeyProfile2Shift.Value, _hotkeyProfile2Alt.Value, ref _wasProfile2Down)) ApplyProfileHotkey(2);
			if (IsHotkeyPressed(hotkeyProfile3, _hotkeyProfile3Ctrl.Value, _hotkeyProfile3Shift.Value, _hotkeyProfile3Alt.Value, ref _wasProfile3Down)) ApplyProfileHotkey(3);
			if (IsHotkeyPressed(hotkeyProfile4, _hotkeyProfile4Ctrl.Value, _hotkeyProfile4Shift.Value, _hotkeyProfile4Alt.Value, ref _wasProfile4Down)) ApplyProfileHotkey(4);

			if (IsHotkeyPressed(hotkeyToggleAuto, _hotkeyToggleAutoCtrl.Value, _hotkeyToggleAutoShift.Value, _hotkeyToggleAutoAlt.Value, ref _wasToggleAutoDown))
			{
				_autoProfileByContext.Set(!_autoProfileByContext.Value);
				_lastHotkeyEvent = "Auto profile -> " + (_autoProfileByContext.Value ? "ON" : "OFF");
			}
		}

		private static void ResetHotkeyEdgeState()
		{
			_wasProfile1Down = false;
			_wasProfile2Down = false;
			_wasProfile3Down = false;
			_wasProfile4Down = false;
			_wasToggleAutoDown = false;
		}

		private static void ResetSmoothingState()
		{
			_smoothedFOV = null;
			_lastSmoothingFrame = -1;
		}

		private static float ApplyFOVSmoothing(float targetFOV)
		{
			if (_enableFOVSmoothing.Value == false)
			{
				_smoothedFOV = targetFOV;
				_lastSmoothingFrame = -1;
				return targetFOV;
			}

			int frameId = -1;
			try
			{
				frameId = ImGui.GetFrameCount();
			}
			catch { }

			if (frameId != -1 && _lastSmoothingFrame == frameId && _smoothedFOV != null)
			{
				return _smoothedFOV.Value;
			}

			float smoothingFactor = Math.Clamp(_fovSmoothingFactor.Value, 0f, 1f);
			if (_smoothedFOV == null || float.IsNaN(_smoothedFOV.Value) || float.IsInfinity(_smoothedFOV.Value))
			{
				_smoothedFOV = targetFOV;
				_lastSmoothingFrame = frameId;
				return targetFOV;
			}

			float smoothed = Mathf.Lerp(_smoothedFOV.Value, targetFOV, smoothingFactor);
			_smoothedFOV = smoothed;
			_lastSmoothingFrame = frameId;
			return smoothed;
		}

		private static void CopyProfileValues(int sourceProfile, int targetProfile)
		{
			if (sourceProfile == targetProfile) return;

			GetTPSFovEntry(targetProfile).SetSilent(GetTPSFovEntry(sourceProfile).Value);
			GetTPSADSFovEntry(targetProfile).SetSilent(GetTPSADSFovEntry(sourceProfile).Value);
			GetFPSFovEntry(targetProfile).SetSilent(GetFPSFovEntry(sourceProfile).Value);
			GetFPSADSFovEntry(targetProfile).SetSilent(GetFPSADSFovEntry(sourceProfile).Value);

			OnSettingsChanged();
		}

		private static float GetTPSFOV(float desiredFOV, int profile)
		{
			//Calculate FOV
			float newFOV = desiredFOV;
			float tpsFOV = GetTPSFovEntry(profile).Value;
			float tpsADSFOV = GetTPSADSFovEntry(profile).Value;
			bool isADS = IsADS();
			bool forceLookExact = _tpsForceExactLookFOV.Value || _tpsForceExactFOV.Value;
			bool forceADSExact = _tpsForceExactADSFOV.Value || _tpsForceExactFOV.Value;

			if (isADS && _tpsDisableADSZoom.Value)
			{
				if (forceLookExact) newFOV = tpsFOV;
				else newFOV = desiredFOV / DEFAULT_TPS_FOV * tpsFOV;
			}
			else if (isADS && forceADSExact)
			{
				newFOV = tpsADSFOV;
			}
			else if (isADS == false && forceLookExact)
			{
				newFOV = tpsFOV;
			}
			else
			{
				if (_tpsFixedADSFOV.Value)
				{
					if (isADS)
					{
						//Scale with ADS FOV
						float t = (desiredFOV - DEFAULT_TPS_FOV) / (DEFAULT_TPS_ADS_FOV - DEFAULT_TPS_FOV);
						t = Mathf.EaseInOutSineLinearBlend(t, _easeStrength.Value);
						newFOV = tpsFOV + t * (tpsADSFOV - tpsFOV);
					}
					else
					{
						//Scale with normal FOV
						newFOV = desiredFOV / DEFAULT_TPS_FOV * tpsFOV;
					}
				}
				else
				{
					//Scale with normal FOV
					newFOV = desiredFOV / DEFAULT_TPS_FOV * tpsFOV;
				}
			}

			return newFOV;
		}

		private static float GetFPSFOV(float desiredFOV, int profile)
		{
			//Calculate FOV
			float newFOV = desiredFOV;
			float fpsFOV = GetFPSFovEntry(profile).Value;
			float fpsADSFOV = GetFPSADSFovEntry(profile).Value;
			bool isADS = IsADS();
			bool forceLookExact = _fpsForceExactLookFOV.Value || _fpsForceExactFOV.Value;
			bool forceADSExact = _fpsForceExactADSFOV.Value || _fpsForceExactFOV.Value;

			if (isADS && _fpsDisableADSZoom.Value)
			{
				if (forceLookExact) newFOV = fpsFOV;
				else newFOV = desiredFOV / DEFAULT_FPS_FOV * fpsFOV;
			}
			else if (isADS && forceADSExact)
			{
				newFOV = fpsADSFOV;
			}
			else if (isADS == false && forceLookExact)
			{
				newFOV = fpsFOV;
			}
			else
			{
				if (_fpsFixedADSFOV.Value)
				{
					if (isADS)
					{
						//Scale with ADS FOV
						float t = (desiredFOV - DEFAULT_FPS_FOV) / (DEFAULT_FPS_ADS_FOV - DEFAULT_FPS_FOV); //Linear interpolation
						t = Mathf.EaseInOutSineLinearBlend(t, _easeStrength.Value); //Sine in out ease
						newFOV = fpsFOV + t * (fpsADSFOV - fpsFOV);
					}
					else
					{
						//Scale with normal FOV
						newFOV = desiredFOV / DEFAULT_FPS_FOV * fpsFOV;
					}
				}
				else
				{
					//Scale with normal FOV
					newFOV = desiredFOV / DEFAULT_FPS_FOV * fpsFOV;
				}
			}

			return newFOV;
		}

		private static void OnSettingsChanged()
		{
			NormalizeSettings();
			_config.SaveToJson();
			ResetSmoothingState();
		}

		private static void RegisterConfigEvents()
		{
			_enabled.ValueChanged += OnSettingsChanged;
			_activeProfile.ValueChanged += OnSettingsChanged;
			_showDebugOverlay.ValueChanged += OnSettingsChanged;
			_autoProfileByContext.ValueChanged += OnSettingsChanged;
			_profileForTPS.ValueChanged += OnSettingsChanged;
			_profileForFPS.ValueChanged += OnSettingsChanged;
			_profileForADS.ValueChanged += OnSettingsChanged;
			_adsProfileHasPriority.ValueChanged += OnSettingsChanged;
			_enableHotkeys.ValueChanged += OnSettingsChanged;
			_hotkeysBlockWhenTyping.ValueChanged += OnSettingsChanged;
			_profileHotkeysForceManual.ValueChanged += OnSettingsChanged;
			_hotkeyProfile1.ValueChanged += OnSettingsChanged;
			_hotkeyProfile2.ValueChanged += OnSettingsChanged;
			_hotkeyProfile3.ValueChanged += OnSettingsChanged;
			_hotkeyProfile4.ValueChanged += OnSettingsChanged;
			_hotkeyToggleAutoProfile.ValueChanged += OnSettingsChanged;
			_hotkeyHoldVanillaFOV.ValueChanged += OnSettingsChanged;
			_hotkeyProfile1Ctrl.ValueChanged += OnSettingsChanged;
			_hotkeyProfile1Shift.ValueChanged += OnSettingsChanged;
			_hotkeyProfile1Alt.ValueChanged += OnSettingsChanged;
			_hotkeyProfile2Ctrl.ValueChanged += OnSettingsChanged;
			_hotkeyProfile2Shift.ValueChanged += OnSettingsChanged;
			_hotkeyProfile2Alt.ValueChanged += OnSettingsChanged;
			_hotkeyProfile3Ctrl.ValueChanged += OnSettingsChanged;
			_hotkeyProfile3Shift.ValueChanged += OnSettingsChanged;
			_hotkeyProfile3Alt.ValueChanged += OnSettingsChanged;
			_hotkeyProfile4Ctrl.ValueChanged += OnSettingsChanged;
			_hotkeyProfile4Shift.ValueChanged += OnSettingsChanged;
			_hotkeyProfile4Alt.ValueChanged += OnSettingsChanged;
			_hotkeyToggleAutoCtrl.ValueChanged += OnSettingsChanged;
			_hotkeyToggleAutoShift.ValueChanged += OnSettingsChanged;
			_hotkeyToggleAutoAlt.ValueChanged += OnSettingsChanged;
			_hotkeyHoldVanillaCtrl.ValueChanged += OnSettingsChanged;
			_hotkeyHoldVanillaShift.ValueChanged += OnSettingsChanged;
			_hotkeyHoldVanillaAlt.ValueChanged += OnSettingsChanged;
			_applyViewModeFromProfile.ValueChanged += OnSettingsChanged;
			_profileViewModeP1.ValueChanged += OnSettingsChanged;
			_profileViewModeP2.ValueChanged += OnSettingsChanged;
			_profileViewModeP3.ValueChanged += OnSettingsChanged;
			_profileViewModeP4.ValueChanged += OnSettingsChanged;

			_tpsFov.ValueChanged += OnSettingsChanged;
			_tpsFovADS.ValueChanged += OnSettingsChanged;
			_tpsFixedADSFOV.ValueChanged += OnSettingsChanged;
			_tpsDisableADSZoom.ValueChanged += OnSettingsChanged;
			_tpsForceExactLookFOV.ValueChanged += OnSettingsChanged;
			_tpsForceExactADSFOV.ValueChanged += OnSettingsChanged;
			_tpsForceExactFOV.ValueChanged += OnSettingsChanged;
			_tpsFovProfile2.ValueChanged += OnSettingsChanged;
			_tpsFovADSProfile2.ValueChanged += OnSettingsChanged;
			_tpsFovProfile3.ValueChanged += OnSettingsChanged;
			_tpsFovADSProfile3.ValueChanged += OnSettingsChanged;
			_tpsFovProfile4.ValueChanged += OnSettingsChanged;
			_tpsFovADSProfile4.ValueChanged += OnSettingsChanged;

			_fpsFOV.ValueChanged += OnSettingsChanged;
			_fpsFOVADS.ValueChanged += OnSettingsChanged;
			_fpsFixedADSFOV.ValueChanged += OnSettingsChanged;
			_fpsDisableADSZoom.ValueChanged += OnSettingsChanged;
			_fpsForceExactLookFOV.ValueChanged += OnSettingsChanged;
			_fpsForceExactADSFOV.ValueChanged += OnSettingsChanged;
			_fpsForceExactFOV.ValueChanged += OnSettingsChanged;
			_fpsFovProfile2.ValueChanged += OnSettingsChanged;
			_fpsFovADSProfile2.ValueChanged += OnSettingsChanged;
			_fpsFovProfile3.ValueChanged += OnSettingsChanged;
			_fpsFovADSProfile3.ValueChanged += OnSettingsChanged;
			_fpsFovProfile4.ValueChanged += OnSettingsChanged;
			_fpsFovADSProfile4.ValueChanged += OnSettingsChanged;

			_easeStrength.ValueChanged += OnSettingsChanged;
			_enableFOVSmoothing.ValueChanged += OnSettingsChanged;
			_fovSmoothingFactor.ValueChanged += OnSettingsChanged;
		}

		private static void UnregisterConfigEvents()
		{
			_enabled.ValueChanged -= OnSettingsChanged;
			_activeProfile.ValueChanged -= OnSettingsChanged;
			_showDebugOverlay.ValueChanged -= OnSettingsChanged;
			_autoProfileByContext.ValueChanged -= OnSettingsChanged;
			_profileForTPS.ValueChanged -= OnSettingsChanged;
			_profileForFPS.ValueChanged -= OnSettingsChanged;
			_profileForADS.ValueChanged -= OnSettingsChanged;
			_adsProfileHasPriority.ValueChanged -= OnSettingsChanged;
			_enableHotkeys.ValueChanged -= OnSettingsChanged;
			_hotkeysBlockWhenTyping.ValueChanged -= OnSettingsChanged;
			_profileHotkeysForceManual.ValueChanged -= OnSettingsChanged;
			_hotkeyProfile1.ValueChanged -= OnSettingsChanged;
			_hotkeyProfile2.ValueChanged -= OnSettingsChanged;
			_hotkeyProfile3.ValueChanged -= OnSettingsChanged;
			_hotkeyProfile4.ValueChanged -= OnSettingsChanged;
			_hotkeyToggleAutoProfile.ValueChanged -= OnSettingsChanged;
			_hotkeyHoldVanillaFOV.ValueChanged -= OnSettingsChanged;
			_hotkeyProfile1Ctrl.ValueChanged -= OnSettingsChanged;
			_hotkeyProfile1Shift.ValueChanged -= OnSettingsChanged;
			_hotkeyProfile1Alt.ValueChanged -= OnSettingsChanged;
			_hotkeyProfile2Ctrl.ValueChanged -= OnSettingsChanged;
			_hotkeyProfile2Shift.ValueChanged -= OnSettingsChanged;
			_hotkeyProfile2Alt.ValueChanged -= OnSettingsChanged;
			_hotkeyProfile3Ctrl.ValueChanged -= OnSettingsChanged;
			_hotkeyProfile3Shift.ValueChanged -= OnSettingsChanged;
			_hotkeyProfile3Alt.ValueChanged -= OnSettingsChanged;
			_hotkeyProfile4Ctrl.ValueChanged -= OnSettingsChanged;
			_hotkeyProfile4Shift.ValueChanged -= OnSettingsChanged;
			_hotkeyProfile4Alt.ValueChanged -= OnSettingsChanged;
			_hotkeyToggleAutoCtrl.ValueChanged -= OnSettingsChanged;
			_hotkeyToggleAutoShift.ValueChanged -= OnSettingsChanged;
			_hotkeyToggleAutoAlt.ValueChanged -= OnSettingsChanged;
			_hotkeyHoldVanillaCtrl.ValueChanged -= OnSettingsChanged;
			_hotkeyHoldVanillaShift.ValueChanged -= OnSettingsChanged;
			_hotkeyHoldVanillaAlt.ValueChanged -= OnSettingsChanged;
			_applyViewModeFromProfile.ValueChanged -= OnSettingsChanged;
			_profileViewModeP1.ValueChanged -= OnSettingsChanged;
			_profileViewModeP2.ValueChanged -= OnSettingsChanged;
			_profileViewModeP3.ValueChanged -= OnSettingsChanged;
			_profileViewModeP4.ValueChanged -= OnSettingsChanged;

			_tpsFov.ValueChanged -= OnSettingsChanged;
			_tpsFovADS.ValueChanged -= OnSettingsChanged;
			_tpsFixedADSFOV.ValueChanged -= OnSettingsChanged;
			_tpsDisableADSZoom.ValueChanged -= OnSettingsChanged;
			_tpsForceExactLookFOV.ValueChanged -= OnSettingsChanged;
			_tpsForceExactADSFOV.ValueChanged -= OnSettingsChanged;
			_tpsForceExactFOV.ValueChanged -= OnSettingsChanged;
			_tpsFovProfile2.ValueChanged -= OnSettingsChanged;
			_tpsFovADSProfile2.ValueChanged -= OnSettingsChanged;
			_tpsFovProfile3.ValueChanged -= OnSettingsChanged;
			_tpsFovADSProfile3.ValueChanged -= OnSettingsChanged;
			_tpsFovProfile4.ValueChanged -= OnSettingsChanged;
			_tpsFovADSProfile4.ValueChanged -= OnSettingsChanged;

			_fpsFOV.ValueChanged -= OnSettingsChanged;
			_fpsFOVADS.ValueChanged -= OnSettingsChanged;
			_fpsFixedADSFOV.ValueChanged -= OnSettingsChanged;
			_fpsDisableADSZoom.ValueChanged -= OnSettingsChanged;
			_fpsForceExactLookFOV.ValueChanged -= OnSettingsChanged;
			_fpsForceExactADSFOV.ValueChanged -= OnSettingsChanged;
			_fpsForceExactFOV.ValueChanged -= OnSettingsChanged;
			_fpsFovProfile2.ValueChanged -= OnSettingsChanged;
			_fpsFovADSProfile2.ValueChanged -= OnSettingsChanged;
			_fpsFovProfile3.ValueChanged -= OnSettingsChanged;
			_fpsFovADSProfile3.ValueChanged -= OnSettingsChanged;
			_fpsFovProfile4.ValueChanged -= OnSettingsChanged;
			_fpsFovADSProfile4.ValueChanged -= OnSettingsChanged;

			_easeStrength.ValueChanged -= OnSettingsChanged;
			_enableFOVSmoothing.ValueChanged -= OnSettingsChanged;
			_fovSmoothingFactor.ValueChanged -= OnSettingsChanged;
		}

		private static void DrawProfileSelector()
		{
			int activeProfile = GetActiveProfileNumber();
			for (int profile = MIN_PROFILE; profile <= MAX_PROFILE; profile++)
			{
				string buttonLabel = "P" + profile + "##ProfileSwitch" + profile;
				if (profile == activeProfile)
				{
					buttonLabel = "[P" + profile + "]##ProfileSwitch" + profile;
				}

				if (ImGui.Button(buttonLabel))
				{
					_activeProfile.Set(profile);
				}
				if (ImGui.IsItemHovered())
				{
					ImGui.SetTooltip("Set active profile to P" + profile + " (" + GetProfileName(profile) + ").");
				}

				if (profile < MAX_PROFILE) ImGui.SameLine();
			}
		}

		private static void DrawProfileCopyButtons(int activeProfile)
		{
			bool hasAnyButton = false;
			for (int profile = MIN_PROFILE; profile <= MAX_PROFILE; profile++)
			{
				if (profile == activeProfile) continue;

				if (hasAnyButton) ImGui.SameLine();
				hasAnyButton = true;
				if (ImGui.Button("Copy active -> P" + profile + "##CopyProfile" + profile))
				{
					CopyProfileValues(activeProfile, profile);
				}
				if (ImGui.IsItemHovered())
				{
					ImGui.SetTooltip("Copy all current FOV values from active profile to P" + profile + ".");
				}
			}
		}

		private static void DrawDebugOverlay()
		{
			if (_showDebugOverlay.Value == false) return;

			bool open = _showDebugOverlay.Value;
			ImGui.SetNextWindowBgAlpha(0.75f);
			ImGuiWindowFlags flags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav;
			if (ImGui.Begin("RE9 Camera Debug", ref open, flags))
			{
				ImGui.Text("Manual profile: P" + GetActiveProfileNumber() + " (" + GetProfileName(GetActiveProfileNumber()) + ")");
				ImGui.Text("Resolved profile: P" + _lastResolvedProfile + " (" + GetProfileName(_lastResolvedProfile) + ")");
				ImGui.Text("Profile source: " + _lastProfileSource);
				ImGui.Text("Hold vanilla: " + (_isHoldingVanillaFOV ? "ACTIVE" : "NO"));
				ImGui.Text("Mode: " + _lastPlayerMode.ToString());
				ImGui.Text("ADS: " + (_lastIsADS ? "YES" : "NO"));
				ImGui.Separator();
				ImGui.Text("Raw FOV: " + _lastRawFOV.ToString("0.00"));
				ImGui.Text("Target FOV: " + _lastTargetFOV.ToString("0.00"));
				ImGui.Text("Applied FOV: " + _lastAppliedFOV.ToString("0.00"));
				ImGui.Text("FPS: " + _lastFramerate.ToString("0"));
				ImGui.Text("Failsafe: " + (_failsafeActive ? "ON" : "off"));
			}
			ImGui.End();

			if (open != _showDebugOverlay.Value)
			{
				_showDebugOverlay.Set(open);
			}
		}

		private static void DrawMiniHud()
		{
			if (_showMiniHud.Value == false) return;

			bool open = true;
			ImGui.SetNextWindowBgAlpha(0.35f);
			ImGuiWindowFlags flags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoInputs;
			if (ImGui.Begin("RE9 VF HUD", ref open, flags))
			{
				ImGui.Text($"P{_lastResolvedProfile} ({GetProfileName(_lastResolvedProfile)}) | {_lastPlayerMode} | ADS {(_lastIsADS ? "Y" : "N")}");
				ImGui.Text($"FOV { _lastAppliedFOV:0}  Smooth {(_failsafeActive || _enableFOVSmoothing.Value == false ? "OFF" : "ON")}  FS {(_failsafeActive ? "ON" : "off")}  FPS {(int)_lastFramerate}");
			}
			ImGui.End();

			if (!open) _showMiniHud.Set(false);
		}

		private static void DrawHelpMarker(string description)
		{
			ImGui.SameLine();
			ImGui.TextDisabled("(?)");
			if (ImGui.IsItemHovered())
			{
				ImGui.BeginTooltip();
				ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35f);
				ImGui.TextUnformatted(description);
				ImGui.PopTextWrapPos();
				ImGui.EndTooltip();
			}
		}



		/* PLUGIN LOAD */
			[PluginEntryPoint]
			private static void Load()
			{
				RegisterConfigEvents();
				_config.LoadFromJson();
				NormalizeSettings();
				ResetSmoothingState();
				ResetHotkeyEdgeState();
				AddonBootstrap.Start();
				Log.Info("Loaded " + VERSION);
			}

		[PluginExitPoint]
			private static void Unload()
			{
				UnregisterConfigEvents();
				ResetHotkeyEdgeState();
				AddonBootstrap.Stop();
				Log.Info("Unloaded " + VERSION);
			}



		/* HOOKS */
		[Callback(typeof(BeginRendering), CallbackType.Pre)]
		public static void PreBeginRendering()
		{
			ProcessHotkeys();
		}

		[MethodHook(typeof(PlayerCameraFOVCalc), nameof(PlayerCameraFOVCalc.getFOV), MethodHookType.Post)]
		public static void PostPlayerCameraFOVCalcGetFOV(ref ulong ptr)
		{
			//Get InteractManager and CharacterManager if null
			if (_interactManager == null) _interactManager = API.GetManagedSingletonT<InteractManager>();
			if (_characterManager == null) _characterManager = API.GetManagedSingletonT<CharacterManager>();

			float rawFOV = BitConverter.Int32BitsToSingle((int)(ptr & 0xFFFFFFFF));
			float targetFOV = rawFOV;
			float appliedFOV = rawFOV;
			bool isADS = IsADS();
			int resolvedProfile = GetActiveProfileNumber();
			string profileSource = "manual";
			bool shouldApplyCustomFOV = _enabled.Value && _isHoldingVanillaFOV == false;
			bool allowSmoothing = _enableFOVSmoothing.Value;

			// FPS monitor + failsafe
			float fps = 0f;
			try { fps = ImGui.GetIO().Framerate; } catch { }
			_lastFramerate = fps;
			_failsafeActive = _failsafeEnabled.Value && fps > 0f && fps < _failsafeFpsThreshold.Value;
			if (_failsafeActive) allowSmoothing = false;

			PlayerMode currentViewMode = _lastPlayerMode;
			if (_characterManager != null && _characterManager.PlayerContextFast != null)
			{
				currentViewMode = _characterManager.PlayerContextFast.CurrentViewMode;
			}

			if (shouldApplyCustomFOV && _autoProfileByContext.Value == false)
			{
				ApplyProfileViewModeIfNeeded(GetActiveProfileNumber(), ref currentViewMode);
			}

			if (shouldApplyCustomFOV)
			{
				resolvedProfile = GetResolvedProfileNumber(currentViewMode, isADS, out profileSource);
				if (currentViewMode == PlayerMode.TPS) targetFOV = GetTPSFOV(rawFOV, resolvedProfile);
				else if (currentViewMode == PlayerMode.FPS) targetFOV = GetFPSFOV(rawFOV, resolvedProfile);

				if (_failsafeActive)
				{
					targetFOV = currentViewMode == PlayerMode.FPS ? _failsafeFPSFOV.Value : _failsafeTPSFOV.Value;
				}

				if (allowSmoothing)
				{
					appliedFOV = ApplyFOVSmoothing(targetFOV);
				}
				else
				{
					ResetSmoothingState();
					appliedFOV = targetFOV;
				}
				ptr = (ptr & 0xFFFFFFFF00000000) | (uint)BitConverter.SingleToInt32Bits(appliedFOV);
			}
			else
			{
				if (_enabled.Value == false) profileSource = "disabled";
				else if (_isHoldingVanillaFOV) profileSource = "hold-vanilla";
				ResetSmoothingState();
			}

			_lastRawFOV = rawFOV;
			_lastTargetFOV = targetFOV;
			_lastAppliedFOV = appliedFOV;
			_lastPlayerMode = currentViewMode;
			_lastIsADS = isADS;
			_lastResolvedProfile = resolvedProfile;
			_lastProfileSource = profileSource;
		}



		/* PLUGIN GENERATED UI */
		[Callback(typeof(ImGuiDrawUI), CallbackType.Pre)]
		public static void PreImGuiDrawUI()
		{
			DrawMiniHud();
			DrawDebugOverlay();

			if (ImGui.TreeNode(GUID_AND_V_VERSION))
			{
				int labelNr = 0;
				int activeProfile = GetActiveProfileNumber();
				ConfigEntry<float> activeTPSFOV = GetTPSFovEntry(activeProfile);
				ConfigEntry<float> activeTPSADSFOV = GetTPSADSFovEntry(activeProfile);
				ConfigEntry<float> activeFPSFOV = GetFPSFovEntry(activeProfile);
				ConfigEntry<float> activeFPSADSFOV = GetFPSADSFovEntry(activeProfile);

				ImGui.TextColored(_colorRed, "ViewForge note: FOV is vertical. 71 vertical ~= 103 horizontal @ 16:9.");
				ImGui.Text("Active: P" + activeProfile + " (" + GetProfileName(activeProfile) + ")");
				ImGui.Text("Resolved: P" + _lastResolvedProfile + " (" + GetProfileName(_lastResolvedProfile) + ")");
				ImGui.Text("Mode: " + _lastPlayerMode + ", ADS: " + (_lastIsADS ? "YES" : "NO"));
				ImGui.Spacing();

				if (ImGui.CollapsingHeader("1) Core"))
				{
					_enabled.DrawCheckbox(); _enabled.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Master switch for the whole mod.");
					_showDebugOverlay.DrawCheckbox(); _showDebugOverlay.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Shows live debug info overlay: profile/mode/FOV values.");
					_showMiniHud.DrawCheckbox(); _showMiniHud.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Tiny HUD: profile/mode/FPS/failsafe.");
					_enableFOVSmoothing.DrawCheckbox(); _enableFOVSmoothing.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Smooths camera FOV transitions.");
					if (_enableFOVSmoothing.Value)
					{
						_fovSmoothingFactor.DrawDragFloat(0.01f, 0f, 1f); _fovSmoothingFactor.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Transition strength: 0 = very slow, 1 = instant.");
					}
					_easeStrength.DrawDragFloat(0.01f, 0f, 1f); _easeStrength.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Curve blend for ADS interpolation in fixed ADS mode.");

					ImGui.Separator();
					_failsafeEnabled.DrawCheckbox(); _failsafeEnabled.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("If FPS ниже порога, сглаживание выкл, FOV -> failsafe.");
					_failsafeFpsThreshold.DrawDragFloat(1f, 5f, 120f); _failsafeFpsThreshold.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Порог FPS для failsafe.");
					_failsafeTPSFOV.DrawDragFloat(0.5f, MIN_FOV, MAX_FOV); _failsafeTPSFOV.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Failsafe FOV для TPS.");
					_failsafeFPSFOV.DrawDragFloat(0.5f, MIN_FOV, MAX_FOV); _failsafeFPSFOV.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Failsafe FOV для FPS.");
					ImGui.Text($"Runtime: FPS {(int)_lastFramerate}, failsafe {(_failsafeActive ? "ON" : "off")}");
				}

				if (ImGui.CollapsingHeader("2) Scene Profiles"))
				{
					DrawProfileSelector();
					_activeProfile.DrawDragInt(1f, MIN_PROFILE, MAX_PROFILE); _activeProfile.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Manual active profile (1-4).");
					_autoProfileByContext.DrawCheckbox(); _autoProfileByContext.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Automatically chooses profile by TPS/FPS/ADS state.");
					if (_autoProfileByContext.Value)
					{
						_profileForTPS.DrawDragInt(1f, MIN_PROFILE, MAX_PROFILE); _profileForTPS.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Profile used in third-person mode.");
						_profileForFPS.DrawDragInt(1f, MIN_PROFILE, MAX_PROFILE); _profileForFPS.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Profile used in first-person mode.");
						_profileForADS.DrawDragInt(1f, MIN_PROFILE, MAX_PROFILE); _profileForADS.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Profile used while aiming down sights.");
						_adsProfileHasPriority.DrawCheckbox(); _adsProfileHasPriority.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("If ON, ADS mapping overrides TPS/FPS mapping.");
					}
					_applyViewModeFromProfile.DrawCheckbox(); _applyViewModeFromProfile.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Applies FPS/TPS mode choice when switching manual profiles.");
					if (_applyViewModeFromProfile.Value)
					{
						DrawProfileViewModeRow(1);
						DrawProfileViewModeRow(2);
						DrawProfileViewModeRow(3);
						DrawProfileViewModeRow(4);
						if (_autoProfileByContext.Value)
						{
							ImGui.TextColored(_colorRed, "Auto profile by context is ON: profile view mode apply is paused.");
						}
					}
					DrawProfileCopyButtons(activeProfile);
				}

				if (ImGui.CollapsingHeader("3) Keybinds"))
				{
					_enableHotkeys.DrawCheckbox(); _enableHotkeys.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Enables all keybind actions in this section.");
					if (_enableHotkeys.Value)
					{
						_hotkeysBlockWhenTyping.DrawCheckbox(); _hotkeysBlockWhenTyping.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Blocks hotkeys while typing in text fields.");
						_profileHotkeysForceManual.DrawCheckbox(); _profileHotkeysForceManual.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Pressing profile hotkey disables Auto Profile mode.");
						bool hotkeysBlockedNow = _hotkeysBlockWhenTyping.Value && IsTypingInUI();
						ImGui.Text("Runtime state: " + (hotkeysBlockedNow ? "BLOCKED by UI input" : "READY"));
						ImGui.Text("Last hotkey event: " + _lastHotkeyEvent);
						DrawHotkeyBindingRow("Profile P1", "Switches active profile to P1.", _hotkeyProfile1, _hotkeyProfile1Ctrl, _hotkeyProfile1Shift, _hotkeyProfile1Alt, ImGuiKey.F1, false, false, false, HOTKEY_BIND_PROFILE_1);
						DrawHotkeyBindingRow("Profile P2", "Switches active profile to P2.", _hotkeyProfile2, _hotkeyProfile2Ctrl, _hotkeyProfile2Shift, _hotkeyProfile2Alt, ImGuiKey.F2, false, false, false, HOTKEY_BIND_PROFILE_2);
						DrawHotkeyBindingRow("Profile P3", "Switches active profile to P3.", _hotkeyProfile3, _hotkeyProfile3Ctrl, _hotkeyProfile3Shift, _hotkeyProfile3Alt, ImGuiKey.F3, false, false, false, HOTKEY_BIND_PROFILE_3);
						DrawHotkeyBindingRow("Profile P4", "Switches active profile to P4.", _hotkeyProfile4, _hotkeyProfile4Ctrl, _hotkeyProfile4Shift, _hotkeyProfile4Alt, ImGuiKey.F4, false, false, false, HOTKEY_BIND_PROFILE_4);
						DrawHotkeyBindingRow("Toggle auto profile", "Turns Auto Profile mode ON/OFF.", _hotkeyToggleAutoProfile, _hotkeyToggleAutoCtrl, _hotkeyToggleAutoShift, _hotkeyToggleAutoAlt, ImGuiKey.F6, false, false, false, HOTKEY_BIND_TOGGLE_AUTO);
						DrawHotkeyBindingRow("Hold vanilla FOV", "While held, temporarily uses game-default FOV.", _hotkeyHoldVanillaFOV, _hotkeyHoldVanillaCtrl, _hotkeyHoldVanillaShift, _hotkeyHoldVanillaAlt, ImGuiKey.LeftAlt, false, false, false, HOTKEY_BIND_HOLD_VANILLA);
					}
					ImGui.TextWrapped("To set a hotkey, hold Set with the left mouse button and press your key combo, or enter the combo manually and click Apply.");
				}

				if (ImGui.CollapsingHeader("4) Shoulder Cam (active profile)"))
				{
					_tpsDisableADSZoom.DrawCheckbox(); _tpsDisableADSZoom.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Keeps normal TPS FOV while ADS.");
					_tpsForceExactLookFOV.DrawCheckbox(); _tpsForceExactLookFOV.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Use exact TPS FOV value (no game scaling).");
					_tpsForceExactADSFOV.DrawCheckbox(); _tpsForceExactADSFOV.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Use exact TPS ADS FOV value (no game scaling).");
					_tpsFixedADSFOV.DrawCheckbox(); _tpsFixedADSFOV.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("ADS uses its own target value instead of percent zoom from base FOV.");
					activeTPSFOV.DrawDragFloat(FOV_STEP, MIN_FOV, MAX_FOV); activeTPSFOV.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Base TPS camera FOV for the active profile.");
					activeTPSADSFOV.DrawDragFloat(FOV_STEP, MIN_FOV, MAX_FOV); activeTPSADSFOV.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("ADS TPS camera FOV for the active profile.");
				}

				if (ImGui.CollapsingHeader("5) Immersive Cam (active profile)"))
				{
					_fpsDisableADSZoom.DrawCheckbox(); _fpsDisableADSZoom.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Keeps normal FPS FOV while ADS.");
					_fpsForceExactLookFOV.DrawCheckbox(); _fpsForceExactLookFOV.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Use exact FPS FOV value (no game scaling).");
					_fpsForceExactADSFOV.DrawCheckbox(); _fpsForceExactADSFOV.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Use exact FPS ADS FOV value (no game scaling).");
					_fpsFixedADSFOV.DrawCheckbox(); _fpsFixedADSFOV.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("ADS uses its own target value instead of percent zoom from base FOV.");
					activeFPSFOV.DrawDragFloat(FOV_STEP, MIN_FOV, MAX_FOV); activeFPSFOV.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("Base FPS camera FOV for the active profile.");
					activeFPSADSFOV.DrawDragFloat(FOV_STEP, MIN_FOV, MAX_FOV); activeFPSADSFOV.DrawResetButtonSameLine(ref labelNr); DrawHelpMarker("ADS FPS camera FOV for the active profile.");
				}

				ImGui.TreePop();
			}
		}
	}

	public static class Extensions
	{
		public static T? TryGetComponent<T>(this GameObject gameObject, string typeName) where T : class
		{
			_System.Type? type = _System.Type.GetType(typeName);
			if (type != null)
			{
				Component? componentFromGameObject = gameObject.getComponent(type);
				if (componentFromGameObject != null)
				{
					if (componentFromGameObject is IObject componentIObject)
					{
						return componentIObject.TryAs<T>();
					}
				}
			}

			return null;
		}
	}

	public static class Mathf
	{
		public static float EaseInOutSineLinearBlend(float t, float k)
		{
			if (t <= 0f) return (1f - k) * t;
			if (t >= 1f) return 1f + (1f - k) * (t - 1f);
			return t + k * ((1f - MathF.Cos(MathF.PI * t)) * 0.5f - t);
		}

		public static float Lerp(float a, float b, float t)
		{
			float clampedT = Math.Clamp(t, 0f, 1f);
			return a + (b - a) * clampedT;
		}
	}

	public static class Log
	{
		private const string PREFIX = "[" + ViewForgePlugin.GUID + "] ";
		public static void Info(string message)
		{
			API.LogInfo(PREFIX + message);
		}

		public static void Warning(string message)
		{
			API.LogWarning(PREFIX + message);
		}

		public static void Error(string message)
		{
			API.LogError(PREFIX + message);
		}
	}

	internal static class ImGuiExtensions
	{
		private static Dictionary<int, string> _resetButtonLabels = new Dictionary<int, string>();

		private static string TryGetResetButtonLabel(ref int labelNr)
		{
			if (_resetButtonLabels.TryGetValue(labelNr, out string? label) == false)
			{
				label = "Reset##" + labelNr;
				_resetButtonLabels[labelNr] = label;
			}

			labelNr++;
			return label;
		}

		public static bool DrawCheckbox(this ConfigEntry<bool> configEntry)
		{
			bool changed = ImGui.Checkbox(configEntry.Key, ref configEntry.RefValue);
			if (changed) configEntry.NotifyValueChanged();

			return changed;
		}

		public static bool DrawDragFloat(this ConfigEntry<float> configEntry, float vSpeed, float vMin, float vMax)
		{
			bool changed = ImGui.DragFloat(configEntry.Key, ref configEntry.RefValue, vSpeed, vMin, vMax);
			if (changed) configEntry.NotifyValueChanged();

			return changed;
		}

		public static bool DrawDragInt(this ConfigEntry<int> configEntry, float vSpeed, int vMin, int vMax)
		{
			int value = configEntry.Value;
			bool changed = ImGui.DragInt(configEntry.Key, ref value, vSpeed, vMin, vMax);
			if (changed) configEntry.Set(value);

			return changed;
		}

		public static bool DrawResetButtonSameLine(this ConfigEntry<bool> configEntry, ref int labelNr)
		{
			float buttonWidth = ViewForgePlugin.ResetTextSize + ImGui.GetStyle().FramePadding.X * 2;

			ImGui.SameLine(ImGui.GetContentRegionAvail().X - buttonWidth);
			bool reset = ImGui.Button(TryGetResetButtonLabel(ref labelNr));
			if (reset) configEntry.Reset();

			return reset;
		}

		public static bool DrawResetButtonSameLine(this ConfigEntry<float> configEntry, ref int labelNr)
		{
			float buttonWidth = ViewForgePlugin.ResetTextSize + ImGui.GetStyle().FramePadding.X * 2;

			ImGui.SameLine(ImGui.GetContentRegionAvail().X - buttonWidth);
			bool reset = ImGui.Button(TryGetResetButtonLabel(ref labelNr));
			if (reset) configEntry.Reset();

			return reset;
		}

		public static bool DrawResetButtonSameLine(this ConfigEntry<int> configEntry, ref int labelNr)
		{
			float buttonWidth = ViewForgePlugin.ResetTextSize + ImGui.GetStyle().FramePadding.X * 2;

			ImGui.SameLine(ImGui.GetContentRegionAvail().X - buttonWidth);
			bool reset = ImGui.Button(TryGetResetButtonLabel(ref labelNr));
			if (reset) configEntry.Reset();

			return reset;
		}
	}
}
