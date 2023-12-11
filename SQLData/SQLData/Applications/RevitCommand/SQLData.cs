using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SQLData.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLData.Applications.RevitCommand
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class SQLData : IExternalCommand
    {

        #region Properties
        // Shared Properties for  Document & UIDocument

        public static Document document { get; set; }
        public static UIDocument uidocument { get; set; }
       
        #endregion

        #region Public methods

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidocument = commandData.Application.ActiveUIDocument;
            document = uidocument.Document;

            try
            {
                SQLDataView sQLDataView = new SQLDataView();
                sQLDataView.ShowDialog();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        #endregion

    }
}
