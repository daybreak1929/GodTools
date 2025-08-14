using System;
using System.Collections.Generic;
using ai.behaviours;
using GodTools.Abstract;
using GodTools.UI;
using HarmonyLib;
using UnityEngine;

namespace GodTools.Features;

public class PrinterEditor : IManager
{
    public void Initialize()
    {
        Harmony.CreateAndPatchAll(typeof(PrinterEditor));
    }
    [HarmonyPrefix, HarmonyPatch(typeof(BehPrinterStep), nameof(BehPrinterStep.printTile))]
    private static bool BehPrinterStep_printTile(Actor pActor)
    {
        if (WindowPrinterEditor.SelectedTileType == null) return true;
        
        
        MusicBox.playSound("event:/SFX/UNIQUE/PrinterStep", pActor.current_tile, false, false);
        MapAction.terraformMain(pActor.current_tile, WindowPrinterEditor.SelectedTileType, AssetManager.terraform.get("destroy"));
        BehaviourActionBase<Actor>.world.setTileDirty(pActor.current_tile);
        BehaviourActionBase<Actor>.world.conway_layer.remove(pActor.current_tile);
        return false;
    }

    internal static void RecalcAllPrintSteps(WindowPrinterEditor.PrintDirection direction)
    {
        foreach (var template in PrintLibrary._instance.list)
        {
            if (!template.name.Contains("quake"))
            {
                switch (direction)
                {
                    case WindowPrinterEditor.PrintDirection.Default:
                        calcSteps(template);
                        break;
                    case WindowPrinterEditor.PrintDirection.InsideToOutside:
                        calcI2OSteps(template);
                        break;
                    case WindowPrinterEditor.PrintDirection.OutsideToInside:
                        calcO2ISteps(template);
                        break;
                }
            }
        }
    }

    private static void calcSteps(PrintTemplate template)
    {
        var print_library = PrintLibrary._instance;
        List<PrintStep> list = new List<PrintStep>();
        int width = (int)(template.graphics.width * WindowPrinterEditor.WidthScale);
        int height = (int)(template.graphics.height * WindowPrinterEditor.HeightScale);
        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < height - 1; j++)
            {
                var ii = (int)(i / WindowPrinterEditor.WidthScale);
                var jj = (int)(j / WindowPrinterEditor.HeightScale);
                ii = Math.Min(ii, template.graphics.width - 1);
                jj = Math.Min(jj, template.graphics.height - 1);
                Color pixel = template.graphics.GetPixel(ii, jj);
                if (pixel == print_library._color_0) continue;
                PrintStep print_step = new PrintStep
                {
                    x = i - 1 - width / 2,
                    y = j - 1 - height / 2,
                    action = 1
                };
                list.Add(print_step);
                if (pixel == print_library._color_2)
                {
                    list.Add(print_step);
                }
                else if (pixel == print_library._color_3)
                {
                    list.Add(print_step);
                    list.Add(print_step);
                }
            }
        }
        template.steps = list.ToArray();
        template.steps_per_tick = (int)((float)template.steps.Length * 0.005f + 1f);
    }
    private static void calcI2OSteps(PrintTemplate template)
    {
    }

    private static void calcO2ISteps(PrintTemplate template)
    {
    }
}