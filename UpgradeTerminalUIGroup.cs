﻿using System;
using System.Collections.Generic;
using System.Text;
using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI;
using InteractiveTerminalAPI.UI.Application;
using BepInEx.Logging;
using InteractiveTerminalAPI.UI.Cursor;
using InteractiveTerminalAPI.UI.Screen;
using static MilkMolars.Plugin;
using UnityEngine.Animations.Rigging;

// https://github.com/WhiteSpike/InteractiveTerminalAPI/wiki/Examples#simple-example-with-code-snippets

namespace MilkMolars
{
    internal class UpgradeTerminalUIGroup : PageApplication
    {
        private static ManualLogSource logger = Plugin.LoggerInstance;
        
        public override void Initialization()
        {
            //MilkMolarController.Init();
            MilkMolarController.InUpgradeUI = false;
            MilkMolarController.InMegaUpgradeUI = true;

            (MilkMolarUpgrade[][], CursorMenu[], IScreen[]) entries = GetPageEntries(NetworkHandler.MegaMilkMolarUpgrades.ToArray());

            MilkMolarUpgrade[][] pagesUpgrades = entries.Item1;
            CursorMenu[] cursorMenus = entries.Item2;
            IScreen[] screens = entries.Item3;

            for (int i = 0; i < pagesUpgrades.Length; i++)
            {
                CursorElement[] elements = new CursorElement[pagesUpgrades[i].Length];

                for (int j = 0; j < elements.Length; j++)
                {
                    if (pagesUpgrades[i][j] == null) continue;

                    MilkMolarUpgrade upgrade = pagesUpgrades[i][j];

                    elements[j] = new CursorElement()
                    {
                        Name = upgrade.GetUpgradeString(),
                        Action = () => BuyUpgrade(upgrade, elements[j])
                    };
                }

                cursorMenus[i] = new CursorMenu()
                {
                    cursorIndex = 0,
                    elements = elements
                };
                CursorMenu cursorMenu = cursorMenus[i];
                screens[i] = new BoxedScreen()
                {
                    Title = "Mega Milk Molar Upgrades", // Title is the text that is displayed in the box on top of the screen
                    elements =
                             [
                                  new TextElement()
                                  {
                                     Text = "These upgrades will affect the entire crew"
                                  },
                                  new TextElement() // This text element is here to give space between the text and the user prompt
                                  {
                                     Text = " "
                                  },
                                  cursorMenu
                             ]
                };
            }

            currentPage = initialPage;
            currentCursorMenu = initialPage.GetCurrentCursorMenu();
            currentScreen = initialPage.GetCurrentScreen();
        }

        protected override int GetEntriesPerPage<T>(T[] entries)
        {
            return 12;
        }

        public void BuyUpgrade(MilkMolarUpgrade upgrade, CursorElement element)
        {
            if (upgrade == null) return;
            logger.LogDebug("BuyUpgrade: " + upgrade.name);

            if (MilkMolarController.BuyMegaMilkMolarUpgrade(upgrade, callRPC: true))
            {
                element.Name = upgrade.GetUpgradeString();
                logger.LogDebug("Bought upgrade " + upgrade.name);
            }
            else
            {
                // TODO: Play error sound
            }
        }
    }
}
