//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Demo_Exzamen
{
    using System;
    using System.Collections.Generic;
    
    public partial class ClientVisit
    {
        public int ID { get; set; }
        public int ClientID { get; set; }
        public System.DateTime VisitDate { get; set; }
        public Nullable<int> ServiceID { get; set; }
    
        public virtual Clients Clients { get; set; }
        public virtual Service Service { get; set; }
    }
}
