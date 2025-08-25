using Fahrenheit.Core.FFX.Atel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraFX.Interop.Windows;

namespace Fahrenheit.Core.FFX2;
public unsafe static class Globals {
    public static class Atel {
        public static int* request_count => FhUtil.ptr_at<int>(0xA116C4);
        public static AtelRequest* request_list => FhUtil.ptr_at<AtelRequest>(0xA12EC8);
        public static AtelWorkerController* controllers => FhUtil.ptr_at<AtelWorkerController>(0xD934D0);
        public static AtelBasicWorker* current_worker => FhUtil.ptr_at<AtelBasicWorker>(0xD94AB0);
        public static AtelWorkerController* current_controller => (AtelWorkerController*)FhUtil.get_at<nint>(0xD94458);
    }

    //public static Btl* btl => FhUtil.ptr_at<Btl>(0x9F77A0); // Maybe?
    public static SaveData* save_data => FhUtil.ptr_at<SaveData>(0x9F9510);

    public static int* event_id => FhUtil.ptr_at<int>(0x962748);
}
