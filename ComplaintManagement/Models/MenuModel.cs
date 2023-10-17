using System.Data;

namespace ComplaintManagement.Models
{
    /// http://www.asparticles.com/2017/10/bind-menu-and-sub-menu-dynamically-in-mvc-from-database.html

    public class MenuModel
    {
        public int menu_order;
        public string? menu_name;
        public string? menu_icon;
        public string? menu_url;
        public string? menu_selected;
        public List<SubMenuModel>? sub_menus;
    }

    public class SubMenuModel
    {
        public int sub_menu_order;
        public string? sub_menu_name;
        public string? sub_menu_url;
        public string? sub_menu_selected;
    }

    public class MenuHandler
    {
        private DBHandler dbhandler;

        public MenuHandler(DBHandler mydbhandler)
        {
            dbhandler = mydbhandler;
        }

        public IList<MenuModel> GetMenu(int profile_id, string pageaccessed)
        {
          
            string sql = "call get_menu (" + profile_id + ", 'main', '');";

            DataTable menudata = dbhandler.GetAdhocData(sql);
            List<MenuModel> menulist = new List<MenuModel>();
            if (menudata.Rows.Count > 0)
            {
                for (int i = 0; i <= menudata.Rows.Count - 1; i++)
                {
                    MenuModel menu = new MenuModel
                    {
                        menu_order = Convert.ToInt32(menudata.Rows[i]["menu_order"].ToString()),
                        menu_name = menudata.Rows[i]["main_menu_name"].ToString(),
                        menu_icon = menudata.Rows[i]["menu_icon"].ToString()
                    };

                    //check if menu has sub menus
                    sql = "call get_menu (" + profile_id + ", 'sub', '" + menudata.Rows[i]["main_menu_name"].ToString() + "');";

                    DataTable submenudata = dbhandler.GetAdhocData(sql);
                    List<SubMenuModel> submenulist = new List<SubMenuModel>();
                    if (submenudata.Rows.Count > 0)
                    {
                        for (int j = 0; j <= submenudata.Rows.Count - 1; j++)
                        {
                            SubMenuModel submenu = new SubMenuModel
                            {
                                sub_menu_order = Convert.ToInt32(submenudata.Rows[j]["sub_menu_order"].ToString()),
                                sub_menu_name = submenudata.Rows[j]["sub_menu_name"].ToString(),
                                sub_menu_url = submenudata.Rows[j]["page_url"].ToString()
                            };

                            //if sub menu is selected then also select main menu
                            if (pageaccessed == submenu.sub_menu_url!.Replace("~", "")!)
                            {
                                submenu.sub_menu_selected = "active";
                                menu.menu_selected = "active";
                            }

                            submenulist.Add(submenu);
                        }
                        menu.menu_url = "#";
                        menu.sub_menus = submenulist;
                    }
                    else
                    {
                        menu.menu_url = dbhandler.GetScalarItem("call get_menu (" + profile_id + ", 'page_url', '" + menu.menu_name + "');");
                        if (pageaccessed == menu.menu_url.Replace("~", ""))
                            menu.menu_selected = "active";
                    }
                    menulist.Add(menu);
                }
            }
            return menulist;
        }
    }
}