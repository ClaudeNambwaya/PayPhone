using System.ComponentModel.DataAnnotations;

namespace ComplaintManagement.Models
{
    public class ReportsModel
    {
        [Key]
        [Display(Name = "Id")]
        public Int16 id { get; set; }

        [Display(Name = "Name"), Required(ErrorMessage = "Value is required.")]
        public String? name { get; set; }

        [Display(Name = "View Name"), Required(ErrorMessage = "Value is required.")]
        public String? view_name { get; set; }

        [Display(Name = "Enabled")]
        public int enabled { get; set; }

        public virtual int created_by { get; set; }
    }

    public class ListFilter
    {
        /// <summary>
        /// Items per page number.
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// Sorting column name.
        /// </summary>
        public string SortColumnName { get; set; }

        /// <summary>
        /// Sorting column number.
        /// </summary>
        public int SortColumn { get; set; }

        /// <summary>
        /// Order direction.
        /// </summary>
        public bool SortOrderAsc { get; set; }

        /// <summary>
        /// List of alphabet indexes.
        /// </summary>
        public List<int> AlphabetFilter { get; set; }

        /// <summary>
        /// List of role unique identifiers.
        /// </summary>
        public List<int> RolesFilter { get; set; }

        /// <summary>
        /// Current page number.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Searching join condition. 
        /// </summary>
        public ListFilterRoleConditon RoleFilterCondition { get; set; }

        /// <summary>
        /// Set of words for searching.
        /// </summary>
        public String Search { get; set; }

        /// <summary>
        /// The class constructor.
        /// </summary>
        public ListFilter()
        {
            RoleFilterCondition = ListFilterRoleConditon.OR;
        }
    }

    public enum ListFilterRoleConditon
    {
        OR = 1,
        AND = 2
    }

    public class ReportListModel
    {
        public List<ReportsModel> Reports { get; set; }
        public ListFilter Filter { get; set; }
        public string HolderName { get; set; }
        public int TotalItems { get; set; }
        //public Role Role { get; set; }
        public bool Assign { get; set; }
        //public ApplicationParameters AppParams { get; private set; }

        public ReportListModel()
        {
            Reports = new List<ReportsModel>();
            //Transactions
            Filter = new ListFilter();
           // AppParams = new ApplicationParameters();
        }

        public ReportListModel(String HolderName, List<ReportsModel> Reports)
        {
            this.HolderName = HolderName;
            this.Reports = Reports;
            Filter = new ListFilter();
          //AppParams = new ApplicationParameters();
        }

        public static List<int> StringToList(String Value)
        {
            int result = 0;
            List<int> Result = new List<int>();

            if (Value != null)
            {
                string[] Values = Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string ItemValue in Values)
                {
                    if (int.TryParse(ItemValue, out result))
                        Result.Add(result);
                }

            }

            return Result;
        }

        public static string ListToString(List<int> Values)
        {
            string Result = "";
            foreach (int ItemValue in Values)
            {
                if (Result.Length > 0)
                    Result += ",";

                Result += ItemValue;
            }

            return Result;
        }

        //public ReportListModel(string HolderName)
        //{
        //    int result = 0;
        //    this.Filter = new ListFilter();
        //   // AppParams = new ApplicationParameters();

        //    int TotalItems = 0;
        //    this.HolderName = HolderName;
        //    //this.Filter.ItemsPerPage = (HttpContext.Current.Request["s"] != null && int.TryParse(HttpContext.Current.Request["s"], out result)) ? result : int.Parse(AppParams.AppUiPagingSize.Value);
        //    this.Filter.CurrentPage = (HttpContext.Current.Request["p"] != null && int.TryParse(HttpContext.Current.Request["p"], out result)) ? result : 1;

        //    this.Filter.SortColumn = (HttpContext.Current.Request["c"] != null && int.TryParse(HttpContext.Current.Request["c"], out result)) ? result : 1;
        //    this.Filter.SortOrderAsc = (HttpContext.Current.Request["o"] != null && (int.TryParse(HttpContext.Current.Request["o"], out result)) ? (result == 1 ? true : false) : true);
        //    this.Filter.SortColumnName = "ID";
        //    this.Filter.Search = (HttpContext.Current.Request["q"] != null ? HttpUtility.UrlDecode(HttpContext.Current.Request["q"]) : "");

        //    if (HttpContext.Current.Request["f"] != null)
        //    {
        //        if (HttpContext.Current.Request["f"] == "or")
        //            Filter.RoleFilterCondition = ListFilterRoleConditon.OR;
        //        else if (HttpContext.Current.Request["f"] == "and")
        //            Filter.RoleFilterCondition = ListFilterRoleConditon.AND;
        //    }

        //    switch (this.Filter.SortColumn)
        //    {
        //        case 2: this.Filter.SortColumnName = "Name"; break;
        //        case 4: this.Filter.SortColumnName = "Email"; break;
        //        case 5: this.Filter.SortColumnName = "LastLogin"; break;
        //        case 6: this.Filter.SortColumnName = "Created"; break;
        //    }

        //    this.Filter.AlphabetFilter = StringToList(HttpContext.Current.Request["a"] != null ? HttpContext.Current.Request["a"] : "");
        //    this.Filter.RolesFilter = StringToList(HttpContext.Current.Request["r"] != null ? HttpContext.Current.Request["r"] : "");

        //    DBHandler mydb = new DBHandler();
        //    this.Reports = mydb.GetReports();// (this.Filter, out TotalItems);
        //    this.TotalItems = TotalItems;
        //}

        public ReportListModel(String HolderName, List<ReportsModel> Reports, ListFilter Filter, int TotalItems)
        {
            this.HolderName = HolderName;
            this.Reports = Reports;
            this.TotalItems = TotalItems;
            this.Filter = Filter;
        }

        public string SortImageClass(int Index)
        {
            string cssClass = "ui-icon-carat-2-n-s";

            if (Index == this.Filter.SortColumn)
            {
                if (this.Filter.SortOrderAsc)
                    cssClass = "ui-icon-triangle-1-n";
                else
                    cssClass = "ui-icon-triangle-1-s";
            }
            return cssClass;
        }
    }
}