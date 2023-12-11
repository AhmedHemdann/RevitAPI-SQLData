
#region NameSpace
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using System.IO;
using System.Windows.Markup;
#endregion


namespace SQLData.Applications
{
    /// <summary>
    /// Setup whole plugins interface with tabs, panels, buttons,...
    /// </summary>
    /// <seealso cref="Autodesk.Revit.UI.IExternalApplication"/>
    public class ExternalApplicationNew : IExternalApplication
    {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// Initializes a new instance of the <see cref="ExternalApplicationNew"/>
        /// </summary>
        public ExternalApplicationNew()
        {

        }
        #endregion

        #region Public methods

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        /// <summary>
        /// Initializes all interface elements on custom created Revit tab.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns></returns>
        public Result OnStartup(UIControlledApplication application)
        {
           
            #region SQL Data

            //Variables
            string RibbonTabName = "ENR Steel";


            //get command dll file path
            string path = Assembly.GetExecutingAssembly().Location;

            //Create Panel
            RibbonPanel DataSQLPanel = application.CreateRibbonPanel(RibbonTabName, "Data Sync");

            // Button Image
            Uri ImagePath = new Uri("pack://application:,,,/FamilyManager;component/Images/Import&Export.png");
            BitmapImage NameImage = new BitmapImage(ImagePath);

            //Create Button
            PushButtonData pushButtonDataSQL = new PushButtonData("SQLData", "SQLData", path, "FamilyManager.Applications.RevitCommand.SQLData")
            {
                LargeImage = NameImage,
                ToolTip = "Export and import Data to a SQL Database",
                LongDescription = "Create a sql table which we can export and import Data to a SQL Database",

            };

            //  Set Url for help to using this ribbon panel
            ContextualHelp contextualHelpDataSQL = new ContextualHelp(ContextualHelpType.Url, "https://www.revitapidocs.com/");
            pushButtonDataSQL.SetContextualHelp(contextualHelpDataSQL);

            DataSQLPanel.AddItem(pushButtonDataSQL);

            return Result.Succeeded;

            #endregion
        }

        #endregion

    }
}



