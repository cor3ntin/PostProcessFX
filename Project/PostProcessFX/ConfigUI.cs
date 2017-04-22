﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using ColossalFramework.UI;
using PostProcessFX.Config;

namespace PostProcessFX
{
    /**
     * The menu that is currently active.
     */
    enum MenuType
    {
        Global,
        Bloom,
        AntiAliasing,
        Motionblur,
        SSAO,
        Reflection,
        DepthOfField
    }

    class ConfigUI : MonoBehaviour
    {
        public const String configFilename = "PostProcessFX_gui_config.xml";

        private String m_toggleKeyString;

        private BloomEffect m_bloom;
        private MotionblurEffect m_motionblur;
        private AntiAliasingEffect m_antiAliasing;

        private GUIConfig m_config = null;
        private MonoBehaviour m_parent;
        private MenuType m_activeMenu;

        private bool m_toggle = false;
        private bool m_dragging = false;
        private float m_lastMouseX;
        private float m_lastMouseY;

        public ConfigUI()
        {
            //  Load the GUIConfig or otherwise return to default settings
            m_config = ConfigUtility.Deserialize<GUIConfig>(configFilename);
            if (m_config == null)
            {
                m_config = GUIConfig.getDefaultConfig();
            }

            // Load the unitypackage with the materials
            AssetBundle assetbundle = AssetBundle.LoadFromFile("PostProcessFX.unitypackage");

            // Create effects from the assetbundle, they should read the required assets by themselves.
            m_bloom = new BloomEffect(assetbundle);
            m_antiAliasing = new AntiAliasingEffect(assetbundle);
            m_motionblur = new MotionblurEffect(assetbundle);
            
            // Obtain the UI toggle key
            m_toggleKeyString = Enum.GetName(typeof(KeyCode), m_config.toggleUIKey);

            // Save the settings at least once
            OnDestroy();
        }

        public void setParent(MonoBehaviour parent)
        {
            m_parent = parent;
        }

        public void OnGUI()
        {
            if (m_config.active)
            {
                drawGUI();
            }
        }

        public void drawGUI()
        {
            float x = m_config.menuPositionX;
            float y = m_config.menuPositionY;

            GUI.Box(new Rect(x, y, 320, 400), "");

            if (GUI.Button(new Rect(x, y, 300, 20), "PostProcessFX UI v1.6.0-f4"))
            {
                m_dragging = true;
            }

            if (GUI.Button(new Rect(x + 300, y, 20, 20), "X"))
            {
                m_config.active = false;
            }

            x += 10;
            y += 25;
            if (GUI.Button(new Rect(x, y, 60, 20), "Global"))
            {
                m_activeMenu = MenuType.Global;
            }

            if (GUI.Button(new Rect(x + 60, y, 60, 20), "Bloom"))
            {
                m_activeMenu = MenuType.Bloom;
            }

            if (GUI.Button(new Rect(x + 120, y, 90, 20), "AntiAliasing"))
            {
                m_activeMenu = MenuType.AntiAliasing;
            }

            if (GUI.Button(new Rect(x + 210, y, 90, 20), "Motionblur"))
            {
                m_activeMenu = MenuType.Motionblur;
            }

            y += 25;
            switch (m_activeMenu)
            {
                case MenuType.Global:
                    GUI.Label(new Rect(x, y, 200, 20), "ToggleUI");
                    y += 25;

                    m_toggleKeyString = GUI.TextArea(new Rect(x, y, 200, 20), m_toggleKeyString);
                    y += 25;

                    m_config.ctrlKey = GUI.Toggle(new Rect(x, y, 50, 20), m_config.ctrlKey, "ctrl");
                    m_config.shiftKey = GUI.Toggle(new Rect(x + 60, y, 50, 20), m_config.shiftKey, "shift");
                    m_config.altKey = GUI.Toggle(new Rect(x + 120, y, 50, 20), m_config.altKey, "alt");

                    try
                    {
                        m_config.toggleUIKey = (int)Enum.Parse(typeof(KeyCode), m_toggleKeyString);
                    }
                    catch (Exception ex)
                    {
                        m_config.toggleUIKey = (int)KeyCode.F9;
                        m_toggleKeyString = "F9";
                    }
                    break;

                case MenuType.AntiAliasing:
                    m_antiAliasing.onGUI(x, y);
                    break;

                case MenuType.Bloom:
                    m_bloom.onGUI(x, y);
                    break;

                case MenuType.Motionblur:
                    m_motionblur.onGUI(x, y);
                    break;
            }
        }

        public void Update()
        {
            bool ctrlMod = false;
            bool altMod = false;
            bool shiftMod = false;

            if (m_config.ctrlKey && (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)))
            {
                ctrlMod = true;
            }

            if (m_config.altKey && (Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftAlt)))
            {
                altMod = true;
            }

            if (m_config.shiftKey && (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)))
            {
                shiftMod = true;
            }

            if (Input.GetKeyDown((KeyCode)m_config.toggleUIKey) &&
                m_config.ctrlKey == ctrlMod &&
                m_config.altKey == altMod &&
                m_config.shiftKey == shiftMod)
            {
                if (!m_toggle)
                {
                    m_config.active = !m_config.active;
                }
                m_toggle = true;
            }

            if (Input.GetKeyUp((KeyCode)m_config.toggleUIKey))
            {
                m_toggle = false;
            }

            var resizeRect = new Rect(m_config.menuPositionX, m_config.menuPositionY, 320, 20);
            var mouse = Input.mousePosition;
            mouse.y = Screen.height - mouse.y;

            if (m_dragging)
            {
                if (!Input.GetMouseButton(0))
                {
                    m_dragging = false;
                }
                else
                {
                    m_config.menuPositionX = mouse.x - m_lastMouseX;
                    m_config.menuPositionY = mouse.y - m_lastMouseY;
                }
            }
            else
            {
                if (Input.GetMouseButton(0) && resizeRect.Contains(mouse))
                {
                    m_dragging = true;
                    m_lastMouseX = mouse.x - m_config.menuPositionX;
                    m_lastMouseY = mouse.y - m_config.menuPositionY;
                }
            }
        }

        public void OnDestroy()
        {
            save();
        }

        public void OnMouseUp()
        {
            m_dragging = false;
        }

        private void save()
        {
            PPFXUtility.log("PostProcessFX: Saving settings...");
            m_bloom.save();
            m_motionblur.save();
            m_antiAliasing.save();

            ConfigUtility.Serialize<GUIConfig>(configFilename, m_config);
        }
    }
}
