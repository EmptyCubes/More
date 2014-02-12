//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Web;
//using System.Web.Configuration;
//using System.Web.Mvc;
//using More.Application.BaseModel;

//namespace More.Web.Designer.Controllers
//{
//    public class ImportController : BaseController
//    {
//        public string ImportDocumentPath
//        {
//            get { return (string)Session["Current_Import_Filename"] ?? null; }
//            set { Session["Current_Import_Filename"] = value; }
//        }

//        public IRulesEngineImporter Importer
//        {
//            get { return Lifeboat.Policy.GetRatingImporters(ImportDocumentPath).First(); }
//        }

//        //
//        // GET: /RatingEngine/Import/
//        public ActionResult Index()
//        {
//            return View();
//        }

//        public ActionResult ImportFiles()
//        {
//            var files = Directory.GetFiles(WebConfigurationManager.AppSettings["RatingImportPath"]).ToArray();
//            return View(files.Select(Path.GetFileName));
//        }

//        public ActionResult SelectFile(string filename)
//        {
//            ImportDocumentPath = Path.Combine(WebConfigurationManager.AppSettings["RatingImportPath"], filename);
//            var model = new ImporterModel();
//            model.Lists = Importer.GetImportLists().Select(p => new ImporterTableModel()
//                                                                    {
//                                                                        Id = p.Name,
//                                                                        Name = p.Name,
//                                                                        NumberRows = p.Rows.Count
//                                                                    });
//            model.Tables = Importer.GetImportTables().Select(p => new ImporterTableModel()
//            {
//                Id = p.Name,
//                Name = p.Name,
//                NumberRows = p.Rows.Count,
//                //TableType = p.
//            });
//            return View("ImporterResults", model);
//        }

//        public ActionResult ChooseFile()
//        {
//            return View();
//        }

//        [HttpPost]
//        public ActionResult ChooseFile(FormCollection form)
//        {
//            HttpPostedFileBase file = Request.Files["file"];
//            var path = ImportDocumentPath = Path.Combine(WebConfigurationManager.AppSettings["RatingImportPath"], file.FileName);
//            file.SaveAs(path);
//            try
//            {
//                return ImportFiles();
//            }
//            catch (Exception ex)
//            {
//                return View("Error", (object)"Error loading document: " + ex.Message);
//            }

//        }

//        [HttpPost]
//        public ActionResult CompleteImport(string[] lists, string[] tables, int exceptionId = 0, bool drop= false)
//        {
//            var importTables = Importer.GetImportTables().Where(p => tables.Contains(p.Name)).ToArray();
//            var importLists = Importer.GetImportLists().Where(p => lists.Contains(p.Name)).ToArray();

//            var errors = new List<string>();

//            try
//            {
//                //foreach (var importList in importLists.Where(p=>lists.Contains(p.Name)))
//                //{
//                //    ConfigStrategy.ImportList(importList);
//                //}
//                // TODO this formerly created lists.  Not sure if its needed anymore since we arn't using the rating engine
//                //foreach (var list in importLists)
//                //{
//                //    foreach (var column in list.KeysAndColumns)
//                //    {
//                //        ConfigStrategy.SaveList(new ListModel()
//                //                                {
//                //                                    Type = 1,
//                //                                    ChangeId = Guid.NewGuid(),
//                //                                    ForeignChangeId = list.Name + "." + column.Name,
//                //                                    Name = list.Name + "." + column.Name
//                //                                });
//                //    }

//                //}
//                TableStrategy.ImportTables(importLists.Concat(importTables), drop, exceptionId);
//            }
//            catch (Exception ex)
//            {
//                errors.Add(ex.Message);
//            }

//            return View("Success", errors);
//        }

//    }
//}