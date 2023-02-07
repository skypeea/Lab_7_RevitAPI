using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using RevitAPITrainingLibrary;
using Lab_7_RevitAPI.Wrappers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lab_7_RevitAPI
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        private Document _doc;

        public List<FamilySymbol> SheetsTypes { get; set; }
        public List<View> Views { get; set; }
        public View SelectedView { get; set; }
        public Element SelectedSheetType { get; set; }
        public int SheetQuantity { get; set; }
        public string DesignedBy { get; set; }
        public DelegateCommand SaveCommand { get; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            _doc = _commandData.Application.ActiveUIDocument.Document;
            SheetsTypes = FamilySymbolUtils.GetTitleBlocks(_commandData);
            Views = ViewsUtils.GetViews(_doc);
            SaveCommand = new DelegateCommand(OnSaveCommand);
        }

        private void OnSaveCommand()
        {
           

            using (var ts = new Transaction(_doc, "Add view sheets"))
            {
                ts.Start();
                for (int i = 0; i < SheetQuantity; i++)
                {
                    ViewSheet viewSheet = ViewSheet.Create(_doc, SelectedSheetType.Id);
                    Parameter designedByParameter = viewSheet.get_Parameter(BuiltInParameter.SHEET_DESIGNED_BY);
                    designedByParameter.Set(DesignedBy);
                    
                    if (SelectedView != null && SheetQuantity == 1)
                    {
                        try {
                            Viewport.Create(_doc, viewSheet.Id, SelectedView.Id, new XYZ());
                        }
                        catch
                        { 
                           
                            TaskDialog.Show("Информация", "Не удается разместить вид на листе");
                        }
                    }
                }
                ts.Commit();
            }
            RaiseCloseRequest();
        }


        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
