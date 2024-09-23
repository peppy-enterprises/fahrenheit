using Fahrenheit.CoreLib;
using System.Collections.Generic;

namespace Fahrenheit.Modules.CSR;

internal unsafe static class Actions {
    public static void init() {
        CSRModule.predicates.AddRange(list);
    }

    private static readonly Dictionary<Func<bool>, Action> list = new() {
        {   // Skip Intro
            () => Globals.save_data->current_room_id == 348 || Globals.save_data->current_room_id == 349,
            () => {
                Globals.save_data->current_room_id = 23;
            }
        },
        {   // Zanarkand Prologue => Tidus's House
            () => Globals.save_data->current_room_id == 132 && Globals.save_data->story_progress == 0,
            () => {
                Globals.save_data->current_room_id = 368;
                Globals.save_data->story_progress = 3;
                Globals.save_data->current_spawnpoint = 0;
            }
        },
        /* Naming Tidus pt 1, 2, 3, 4, 5, and 6 go here*/
        { // Tidus looking at Jecht sign
            () => Globals.save_data->current_room_id == 376 && Globals.save_data->story_progress == 4,
            () => {
                Globals.save_data->story_progress = 5;
                Globals.save_data->current_spawnpoint = 0;
            }
        },
        { // Blitzball FMV
            () => Globals.save_data->current_room_id == 371 && Globals.save_data->story_progress == 6,
            () => {
                Globals.save_data->current_room_id = 370;
                Globals.save_data->story_progress = 7;
                Globals.save_data->current_spawnpoint = 0;
            }
        }
    };
}
