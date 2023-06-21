using System;
using System.Diagnostics;
using System.Linq;

using FFXCutsceneRemover.ComponentUtil;

namespace FFXCutsceneRemover;

class FarplaneTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;
        int baseAddress = MemoryWatchers.GetBaseAddress();


        if (Stage == 0)
        {
            base.Execute();

            BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;

            Stage += 1;
        }
        else if (MemoryWatchers.FarplaneTransition1.Current == (BaseCutsceneValue + 0XB12D) && Stage == 1)
        {
            WriteValue<int>(MemoryWatchers.FarplaneTransition1, BaseCutsceneValue + 0XB350);

            Stage += 1;
        }
        else if (MemoryWatchers.FarplaneTransition2.Current == (BaseCutsceneValue + 0xB620) && Stage == 2)
        {
            WriteValue<int>(MemoryWatchers.FarplaneTransition2, BaseCutsceneValue + 0xB86C);

            process.Suspend();
            IntPtr EquipMenu = new IntPtr(baseAddress + 0xD30F2C); // Address of beginning of Equipment menu
            bool foundBrotherhood = false;
            var brotherhood = new byte[2] { 0x1, 0x50 }; // Brotherhood name identifier in hex

            while (!foundBrotherhood)
            {
                // Check first two bytes for name identifier and compare against Brotherhood
                var equipment = process.ReadBytes(EquipMenu, 2);

                if (equipment.SequenceEqual<byte>(brotherhood))
                {
                    // Not sure what this value is, but it does change during the scene, so adding just in case!
                    IntPtr aNumber = IntPtr.Add(EquipMenu, 3);
                    process.WriteBytes(aNumber, new byte[1] { 0x9 });

                    // Second slot for Brotherhood, +10% Strength
                    IntPtr slot2 = IntPtr.Add(EquipMenu, 16);
                    process.WriteBytes(slot2, new byte[2] { 0x64, 0x80 });

                    // Third slot for Brotherhood, Waterstrike
                    IntPtr slot3 = IntPtr.Add(EquipMenu, 18);
                    process.WriteBytes(slot3, new byte[2] { 0x2A, 0x80 });

                    // Fourth slot for Brotherhood, Sensor
                    IntPtr slot4 = IntPtr.Add(EquipMenu, 20);
                    process.WriteBytes(slot4, new byte[2] { 0x00, 0x80 });

                    // Finally skip the Farplane scenes
                    foundBrotherhood = true;
                    process.Resume();
                }
                else
                {
                    // Number of bytes for each piece of equipment is 22, so if not found, go to the next piece of equipment
                    EquipMenu = IntPtr.Add(EquipMenu, 22);
                }
            }

            Stage += 1;
        }
        else if (MemoryWatchers.FarplaneTransition2.Current == (BaseCutsceneValue + 0xBC00) && Stage == 3)
        {
            new Transition { RoomNumber = 134, Storyline = 1170, TidusWeaponDamageBoost = 15, Description = "Tidus talks to Yuna" }.Execute();
            Stage += 1;
        }
    }
}