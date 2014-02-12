using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using More.Application.BaseModel;
using More.Application.Entity.Repository;
using More.Engine.Model;

namespace More.UnitTests
{
    [TestClass]
    public class LookupTableRepositoryTests
    {
        private IRatingEngineTableRepository _repository;
        private IRatingEngineTableRepository _multipleRepository;

        [TestInitialize]
        public void Initialize()
        {
            _repository =
                new SqlLookupTableRepository(
                    new SqlLookupTableContext(
                        ConfigurationManager.ConnectionStrings["Dev.IsoDb"].ConnectionString),
                    new DateTime(2015, 10, 1), false);

            _multipleRepository = new SqlLookupTableRepository(
                new Dictionary<string, ILookupTableContext>
                {
                    {
                        "ISO",
                        new SqlLookupTableContext(ConfigurationManager.ConnectionStrings["Dev.IsoDb"].ConnectionString)
                    },
                    {
                        "RuleEngineUser",
                        new SqlLookupTableContext(
                            ConfigurationManager.ConnectionStrings["Dev.RuleEngineDb"].ConnectionString)
                    }
                },
                new DateTime(2015, 10, 1), false);
        }

        [TestMethod]
        public void GetLookupTableByNameMultipleDatabases()
        {
            const string tableName1 = "PrivatePassengerOtherThanCollisionDeductibleRelativity";
            const string schemaName1 = "ISO_AK";

            const string tableName2 = "CCI_AgeTier";
            const string schemaName2 = "RuleEngineUser";

            var repository = new SqlLookupTableRepository(
                new Dictionary<string, ILookupTableContext>
                {
                    {
                        "ISO",
                        new SqlLookupTableContext(ConfigurationManager.ConnectionStrings["Dev.IsoDb"].ConnectionString)
                    },
                    {
                        "RuleEngineUser",
                        new SqlLookupTableContext(
                            ConfigurationManager.ConnectionStrings["Dev.RuleEngineDb"].ConnectionString)
                    }
                },
                new DateTime(2015, 10, 1), false);

            var table = repository.GetLookupTable(tableName1, schemaName1, null);
            Assert.IsNotNull(table);
            Assert.AreEqual(table.Name, tableName1);

            table = repository.GetLookupTable(tableName2, schemaName2, null);
            Assert.IsNotNull(table);
            Assert.AreEqual(table.Name, tableName2);
        }

        [TestMethod]
        public void GetRatingTablesMultipleDatabases()
        {
            var tables = _multipleRepository.GetRatingTables();
            Assert.IsTrue(tables.Any());
            Assert.IsTrue(tables.Any(t => t.Name == "CCI_AgeTier"));
        }

        [TestMethod]
        public void GetLookupTableMultipleDatabases()
        {
            const string tableName1 = "PrivatePassengerOtherThanCollisionDeductibleRelativity";
            const string tableId1 = "6c84c211-032b-4531-a33b-a083b8a4406b";

            const string tableName2 = "CCI_AgeTier";
            const string tableId2 = "8abbb5c8-2a0b-49cc-9369-1ab20f745f4a";

            var table = _multipleRepository.GetLookupTable(tableId1);
            Assert.IsTrue(table != null);
            Assert.IsTrue(table.Name == tableName1);


            table = _multipleRepository.GetLookupTable(tableId2);
            Assert.IsTrue(table != null);
            Assert.IsTrue(table.Name == tableName2);
        }

        [TestMethod]
        public void GetLookupTableModelMultipleDatabases()
        {
            const string tableToMatch = "PrivatePassengerOtherThanCollisionDeductibleRelativity";
            const string tableId = "6C84C211-032B-4531-A33B-A083B8A4406B";

            const string tableName2 = "CCI_AgeTier";
            const string tableId2 = "8abbb5c8-2a0b-49cc-9369-1ab20f745f4a";

            var table = _multipleRepository.GetRatingTableModel(tableId);

            Assert.IsNotNull(table);
            Assert.AreEqual(tableToMatch, table.Name);

            table = _multipleRepository.GetRatingTableModel(tableId2);
            Assert.IsNotNull(table);
            Assert.AreEqual(tableName2, table.Name);
        }

        [TestMethod]
        public void GetLookupTableRowMultipleDatabases()
        {
            const string tableId = "6C84C211-032B-4531-A33B-A083B8A4406B";
            const string rowId = "7";

            var row = _multipleRepository.GetLookupTableRow(string.Empty, tableId, rowId);

            Assert.IsNotNull(row);
            Assert.AreEqual("-0.1600", row["Factor"].ToString());
            Assert.AreEqual("250", row["OtherThanCollisionDeductible"].ToString());

            const string tableId2 = "8abbb5c8-2a0b-49cc-9369-1ab20f745f4a";
            const string rowId2 = "5";

            row = _multipleRepository.GetLookupTableRow(string.Empty, tableId2, rowId2);

            Assert.IsNotNull(row);
            Assert.AreEqual("55", row["Age"].ToString());
            Assert.AreEqual("1", row["Tier"].ToString());
        }

        [TestMethod]
        public void GetLookupTableKeyMultipleDatabases()
        {
            const string tableName = "PrivatePassengerOtherThanCollisionDeductibleRelativity";
            const string tableId = "6C84C211-032B-4531-A33B-A083B8A4406B";
            const string keyColumnName = "OtherThanCollisionDeductible";
            const string tableSchema = "ISO_AK";

            const string tableName2 = "CCI_AgeTier";
            const string tableId2 = "8abbb5c8-2a0b-49cc-9369-1ab20f745f4a";
            const string keyColumnName2 = "Age";
            const string tableSchema2 = "RuleEngineUser";

            var key = _multipleRepository.GetLookupTableKey(string.Empty, tableId, keyColumnName);

            Assert.IsTrue(key != null);
            Assert.IsTrue(key.Name == keyColumnName);
            Assert.IsTrue(key.LookupType == LookupType.ExactMatch);
            Assert.IsTrue(key.TableSchema == tableSchema);
            Assert.IsTrue(key.TableName == tableName);

            key = _multipleRepository.GetLookupTableKey(string.Empty, tableId2, keyColumnName2);

            Assert.IsTrue(key != null);
            Assert.IsTrue(key.Name == keyColumnName2);
            Assert.IsTrue(key.LookupType == LookupType.LessThanOrEqual);
            Assert.IsTrue(key.TableSchema == tableSchema2);
            Assert.IsTrue(key.TableName == tableName2);
        }

        [TestMethod]
        public void SaveLookupTableKeyMultipleDatabases()
        {
            const string tableId = "6C84C211-032B-4531-A33B-A083B8A4406B";
            const string keyColumnName = "OtherThanCollisionDeductible";

            var key = _multipleRepository.GetLookupTableKey(string.Empty, tableId, keyColumnName);
            Assert.IsTrue(key != null);
            var originalKeyLookupType = key.LookupType;

            key.LookupType = LookupType.GreaterThan;
            _multipleRepository.SaveLookupTableKey(key);

            var updatedKey = _multipleRepository.GetLookupTableKey(string.Empty, tableId, keyColumnName);
            Assert.IsTrue(updatedKey != null);
            Assert.IsTrue(originalKeyLookupType != updatedKey.LookupType);

            const string tableId2 = "8abbb5c8-2a0b-49cc-9369-1ab20f745f4a";
            const string keyColumnName2 = "Age";

            key = _multipleRepository.GetLookupTableKey(string.Empty, tableId2, keyColumnName2);
            Assert.IsTrue(key != null);
            originalKeyLookupType = key.LookupType;

            key.LookupType = LookupType.ExactMatch;
            _multipleRepository.SaveLookupTableKey(key);

            updatedKey = _multipleRepository.GetLookupTableKey(string.Empty, tableId2, keyColumnName2);
            Assert.IsTrue(updatedKey != null);
            Assert.IsTrue(originalKeyLookupType != updatedKey.LookupType);
        }

        [TestMethod]
        public void DeleteLookupTableRowMultipleDatabases()
        {
            const string tableId1 = "6C84C211-032B-4531-A33B-A083B8A4406B";
            const string tableId2 = "8abbb5c8-2a0b-49cc-9369-1ab20f745f4a";

            //get the original, delete the row, get the updated table
            var originalTable = _multipleRepository.GetLookupTable(tableId1);
            Assert.IsNotNull(originalTable);

            _multipleRepository.ChangeEffectiveDate(new DateTime(2013, 4, 1));
            _multipleRepository.DeleteLookupTableRow(originalTable.Name, tableId1, "8");
            var updatedTable = _multipleRepository.GetLookupTable(tableId1);
            Assert.IsNotNull(updatedTable);

            Assert.IsTrue(originalTable.Rows.Single(r => r.RowId == "8").Active);
            Assert.IsTrue(!updatedTable.Rows.Single(r => r.RowId == "8").Active);

            //second database
            _multipleRepository.ChangeEffectiveDate(new DateTime(2013, 2, 20));
            originalTable = _multipleRepository.GetLookupTable(tableId2);
            Assert.IsNotNull(originalTable);

            _multipleRepository.DeleteLookupTableRow(originalTable.Name, tableId2, "9");
            updatedTable = _multipleRepository.GetLookupTable(tableId2);
            Assert.IsNotNull(updatedTable);

            Assert.IsTrue(originalTable.Rows.Single(r => r.RowId == "9").Active);
            Assert.IsTrue(!updatedTable.Rows.Single(r => r.RowId == "9").Active);
        }

        [TestMethod]
        public void UpdateLookupTableRowMultipleDatabases()
        {
            const string tableId = "6C84C211-032B-4531-A33B-A083B8A4406B";
            const string rowId = "8";

            _multipleRepository.ChangeEffectiveDate(new DateTime(2013, 4, 1));

            var row = _multipleRepository.GetLookupTableRow(string.Empty, tableId, rowId);
            Assert.IsNotNull(row);

            var modifiedRow = row;

            
            modifiedRow.Values[5] = "true"; //Active
            modifiedRow.Values[6] = "true"; //IsOverride
            modifiedRow.Values[7] = "251"; //OtherThanCollisionDeductible

            _multipleRepository.SaveLookupTableRow(modifiedRow);

            var updatedRow = _multipleRepository.GetLookupTableRow(string.Empty, tableId, rowId);
            Assert.IsNotNull(updatedRow);

            //Assert.IsTrue(row.IsOverride != updatedRow.IsOverride);
            Assert.IsTrue(updatedRow.Active);

            const string tableId2 = "8abbb5c8-2a0b-49cc-9369-1ab20f745f4a";
            const string rowId2 = "9";

            _multipleRepository.ChangeEffectiveDate(new DateTime(2013, 2, 20));

            row = _multipleRepository.GetLookupTableRow(string.Empty, tableId2, rowId2);
            Assert.IsNotNull(row);

            modifiedRow = row;

            modifiedRow.Values[5] = "true"; //Active
            modifiedRow.Values[6] = "true"; //IsOverride
            modifiedRow.Values[7] = "998"; //Age

            _multipleRepository.SaveLookupTableRow(modifiedRow);

            updatedRow = _multipleRepository.GetLookupTableRow(string.Empty, tableId2, rowId2);
            Assert.IsNotNull(updatedRow);

            Assert.IsTrue(updatedRow["Age"].ToString() == "998");
        }

        [TestMethod]
        public void InsertLookupTableRowMultipleDatabases()
        {
            const string tableId = "6C84C211-032B-4531-A33B-A083B8A4406B";

            var row = new LookupTableRowModel
            {
                TableId = tableId,
                EffectiveDate = new DateTime(2013, 10, 1),
                IsOverride = true,
                Active = true
            };

            row.Values = new[]
            {
                "0", //rowId
                "0", //sequence
                Guid.Empty.ToString(), //changeId
                row.EffectiveDate.ToString(CultureInfo.InvariantCulture), //effectiveDate
                DateTime.MaxValue.ToString(CultureInfo.InvariantCulture), //expirationDate
                "true", //active
                "true", //override
                "4,999", //keycol1
                "BFS", //writingcompany
                "0.4999" //valcol1
            };

            _multipleRepository.SaveLookupTableRow(row);
            var lookupTable = _multipleRepository.GetLookupTable(tableId);
            var insertedRow = lookupTable.Rows.Last();

            Assert.IsTrue(insertedRow.KeyValues.First().LowValue == row.Values[7]);
            Assert.IsTrue(insertedRow.ColumnValues["Factor"].ToString() == row.Values[9]);

            const string tableId2 = "8abbb5c8-2a0b-49cc-9369-1ab20f745f4a";

            row = new LookupTableRowModel
            {
                TableId = tableId2,
                EffectiveDate = new DateTime(2014, 1, 1),
                IsOverride = true,
                Active = true
            };

            row.Values = new[]
            {
                "0",
                "0",
                Guid.Empty.ToString(),
                row.EffectiveDate.ToString(CultureInfo.InvariantCulture),
                DateTime.MaxValue.ToString(CultureInfo.InvariantCulture),
                "true",
                "true",
                "999",
                "5"
            };

            _multipleRepository.SaveLookupTableRow(row);
            lookupTable = _multipleRepository.GetLookupTable(tableId2);
            insertedRow = lookupTable.Rows.Last();

            Assert.IsTrue(insertedRow.KeyValues.First().LowValue == row.Values[7]);
            Assert.IsTrue(insertedRow.ColumnValues["Tier"].ToString() == row.Values[8]);
        }

        [TestMethod]
        public void CreateAndDropTableMultipleDatabases()
        {
            var table = new LookupTable("TestTable");
            var keyColumns = new[]
            {
                new FactorTableColumnDefinition("ColumnA", true)
                {
                    LookupType = LookupType.ExactMatch
                },
                new FactorTableColumnDefinition("ColumnB", true)
                {
                    LookupType = LookupType.ExactMatch
                }
            };
            var dataColumns = new[]
            {
                new FactorTableColumnDefinition("IntColumn", false)
                {
                    DataType = typeof(int)
                },
                new FactorTableColumnDefinition("DecimalColumn", false)
                {
                    DataType = typeof(decimal)
                }
            };
            table.Keys = keyColumns;
            table.Columns = dataColumns;
            table.Properties.Schema = "dbo";
            table.Properties.Context = "ISO";

            _multipleRepository.ImportTables(new[] { table }, false);

            var addedTable =
                _multipleRepository.GetRatingTables(
                    t => t.Name.Equals("TestTable", StringComparison.InvariantCultureIgnoreCase) &&
                         t.Properties.Schema == "dbo").FirstOrDefault();

            Assert.IsTrue(addedTable != null);

            _multipleRepository.DropTables(new[] { _multipleRepository.GetLookupTable(addedTable.ChangeId) });

            var noTable = _multipleRepository.GetRatingTables(
                    t => t.Name.Equals("TestTable", StringComparison.InvariantCultureIgnoreCase) &&
                         t.Properties.Schema == "dbo");

            Assert.IsTrue(!noTable.Any());
        }

        #region OldTests
        //[TestMethod]
        //public void GetRatingTables()
        //{
        //    var tables = _repository.GetRatingTables();
        //    Assert.IsTrue(tables.Any());
        //}

        //[TestMethod]
        //public void GetRatingTablesPredicate()
        //{
        //    var tables = _repository.GetRatingTables(t =>
        //        t.Properties.Schema == "ISO_AL" ||
        //        t.Properties.Schema == "ISO_CW" && t.Properties.EffectiveDate <= new DateTime(2016, 10, 1));

        //    Assert.IsTrue(tables != null);

        //    var lookupTables = tables.ToList();
        //    Assert.IsTrue(lookupTables.Any());
        //    Assert.IsTrue(lookupTables.Count() == 175);
        //}

        //[TestMethod]
        //public void GetLookupTable()
        //{
        //    const string tableToMatch = "PrivatePassengerOtherThanCollisionDeductibleRelativity";
        //    const string tableId = "6C84C211-032B-4531-A33B-A083B8A4406B";

        //    var table = _repository.GetLookupTable(tableId);

        //    Assert.IsNotNull(table);
        //    Assert.AreEqual(tableToMatch, table.Name);
        //}

        //[TestMethod]
        //public void GetLookupTableModel()
        //{
        //    const string tableToMatch = "PrivatePassengerOtherThanCollisionDeductibleRelativity";
        //    const string tableId = "6C84C211-032B-4531-A33B-A083B8A4406B";

        //    var table = _repository.GetRatingTableModel(tableId);

        //    Assert.IsNotNull(table);
        //    Assert.AreEqual(tableToMatch, table.Name);
        //}

        //[TestMethod]
        //public void GetLookupTableRow()
        //{
        //    const string tableId = "6C84C211-032B-4531-A33B-A083B8A4406B";
        //    const string rowId = "7";

        //    var row = _repository.GetLookupTableRow(string.Empty, tableId, rowId);

        //    Assert.IsNotNull(row);
        //    Assert.AreEqual("-0.1600", row["Factor"].ToString());
        //    Assert.AreEqual("250", row["OtherThanCollisionDeductible"].ToString());
        //}

        //[TestMethod]
        //public void DeleteLookupTableRow()
        //{
        //    const string tableId = "6C84C211-032B-4531-A33B-A083B8A4406B";

        //    //get the original, delete the row, get the updated table
        //    var originalTable = _repository.GetLookupTable(tableId);
        //    Assert.IsNotNull(originalTable);

        //    _repository.DeleteLookupTableRow(originalTable.Name, tableId, "8");
        //    var updatedTable = _repository.GetLookupTable(tableId);
        //    Assert.IsNotNull(updatedTable);

        //    Assert.IsTrue(originalTable.Rows.Single(r => r.RowId == "8").Active);
        //    Assert.IsTrue(!updatedTable.Rows.Single(r => r.RowId == "8").Active);
        //}

        //[TestMethod]
        //public void DeleteLookupTableRowDifferentEffectiveDate()
        //{
        //    var repository = new SqlLookupTableRepository(
        //            new SqlLookupTableContext(
        //                ConfigurationManager.ConnectionStrings["Dev.IsoDb"].ConnectionString),
        //            new DateTime(2013, 11, 1), false);

        //    const string tableId = "6C84C211-032B-4531-A33B-A083B8A4406B";

        //    //get the original, delete the row, get the updated table
        //    var originalTable = repository.GetLookupTable(tableId);
        //    Assert.IsNotNull(originalTable);

        //    repository.DeleteLookupTableRow(originalTable.Name, tableId, "8");
        //    var updatedTable = repository.GetLookupTable(tableId);
        //    Assert.IsNotNull(updatedTable);
        //}

        //[TestMethod]
        //public void UpdateLookupTableRow()
        //{
        //    const string tableId = "6C84C211-032B-4531-A33B-A083B8A4406B";
        //    const string rowId = "8";

        //    var row = _repository.GetLookupTableRow(string.Empty, tableId, rowId);
        //    Assert.IsNotNull(row);

        //    var modifiedRow = row;

        //    modifiedRow.Values[7] = "3,999";
        //    modifiedRow.Values[6] = "true";

        //    _repository.SaveLookupTableRow(modifiedRow);

        //    var updatedRow = _repository.GetLookupTableRow(string.Empty, tableId, rowId);
        //    Assert.IsNotNull(updatedRow);

        //    Assert.IsTrue(row.IsOverride != updatedRow.IsOverride);
        //}

        //[TestMethod]
        //public void InsertLookupTableRow()
        //{
        //    const string tableId = "6C84C211-032B-4531-A33B-A083B8A4406B";

        //    var row = new LookupTableRowModel
        //    {
        //        TableId = tableId,
        //        EffectiveDate = new DateTime(2013, 10, 1),
        //        IsOverride = true,
        //        Active = true
        //    };

        //    row.Values = new[]
        //    {
        //        "0", //rowId
        //        "0", //sequence
        //        Guid.Empty.ToString(), //changeId
        //        row.EffectiveDate.ToString(CultureInfo.InvariantCulture), //effectiveDate
        //        DateTime.MaxValue.ToString(CultureInfo.InvariantCulture), //expirationDate
        //        "true", //active
        //        "true", //override
        //        "4,999", //keycol1
        //        "0.4999" //valcol1
        //    };

        //    _repository.SaveLookupTableRow(row);
        //    var lookupTable = _repository.GetLookupTable(tableId);
        //    var insertedRow = lookupTable.Rows.Last();

        //    Assert.IsTrue(insertedRow.KeyValues.First().LowValue == row.Values[7]);
        //    Assert.IsTrue(insertedRow.ColumnValues.First().Value.ToString() == row.Values[8]);
        //}

        //[TestMethod]
        //public void SoftDropTable()
        //{
        //    const string tableId = "6C84C211-032B-4531-A33B-A083B8A4406B";
        //    var table = _repository.GetLookupTable(tableId);
        //    Assert.IsTrue(table.Active);

        //    table.Properties.Active = false;

        //    var tableList = new List<LookupTable>
        //    {
        //        table
        //    };

        //    _repository.ImportTables(tableList, true);

        //    var updatedTable = _repository.GetLookupTable(tableId);

        //    Assert.IsTrue(!updatedTable.Active);
        //}

        //[TestMethod]
        //public void CreateTable()
        //{
        //    var setupRepository = new SqlLookupTableRepository(
        //            new SqlLookupTableContext(
        //                ConfigurationManager.ConnectionStrings["Dev.IsoDb"].ConnectionString),
        //            new DateTime(2013, 11, 1), true);

        //    var table = new LookupTable("TestTable");
        //    var keyColumns = new[]
        //    {
        //        new FactorTableColumnDefinition("ColumnA", true)
        //        {
        //            LookupType = LookupType.ExactMatch
        //        },
        //        new FactorTableColumnDefinition("ColumnB", true)
        //        {
        //            LookupType = LookupType.ExactMatch
        //        }
        //    };
        //    var dataColumns = new[]
        //    {
        //        new FactorTableColumnDefinition("IntColumn", false)
        //        {
        //            DataType = typeof(int)
        //        },
        //        new FactorTableColumnDefinition("DecimalColumn", false)
        //        {
        //            DataType = typeof(decimal)
        //        }
        //    };
        //    table.Keys = keyColumns;
        //    table.Columns = dataColumns;
        //    table.Properties.Schema = "ISO_CA";

        //    setupRepository.ImportTables(new[] { table }, false);

        //    var addedTable =
        //        setupRepository.GetRatingTables(
        //            t => t.Name.Equals("TestTable", StringComparison.InvariantCultureIgnoreCase) &&
        //                 t.Properties.Schema == "ISO_CA");

        //    Assert.IsTrue(addedTable != null);
        //}

        //[TestMethod]
        //public void CreateTableInsertRowHardDeleteRow()
        //{
        //    var setupRepository = new SqlLookupTableRepository(
        //            new SqlLookupTableContext(
        //                ConfigurationManager.ConnectionStrings["Dev.IsoDb"].ConnectionString),
        //            new DateTime(2013, 11, 1), true);

        //    var table = new LookupTable("TestTable");
        //    var keyColumns = new[]
        //    {
        //        new FactorTableColumnDefinition("ColumnA", true)
        //        {
        //            LookupType = LookupType.ExactMatch
        //        },
        //        new FactorTableColumnDefinition("ColumnB", true)
        //        {
        //            LookupType = LookupType.ExactMatch
        //        }
        //    };
        //    var dataColumns = new[]
        //    {
        //        new FactorTableColumnDefinition("IntColumn", false)
        //        {
        //            DataType = typeof(int)
        //        },
        //        new FactorTableColumnDefinition("DecimalColumn", false)
        //        {
        //            DataType = typeof(decimal)
        //        }
        //    };
        //    table.Keys = keyColumns;
        //    table.Columns = dataColumns;
        //    table.Properties.Schema = "ISO_AK";

        //    setupRepository.ImportTables(new[] { table }, false);

        //    var addedTable =
        //        setupRepository.GetRatingTables(
        //            t => t.Name.Equals("TestTable", StringComparison.InvariantCultureIgnoreCase) &&
        //                 t.Properties.Schema == "ISO_AK").First();

        //    var row = new LookupTableRowModel
        //    {
        //        TableId = addedTable.ChangeId,
        //        EffectiveDate = new DateTime(2013, 11, 1),
        //        IsOverride = true,
        //        Active = true
        //    };

        //    row.Values = new[]
        //    {
        //        "0", //rowId
        //        "0", //sequence
        //        Guid.Empty.ToString(), //changeId
        //        row.EffectiveDate.ToString(CultureInfo.InvariantCulture), //effectiveDate
        //        DateTime.MaxValue.ToString(CultureInfo.InvariantCulture), //expirationDate
        //        "true", //active
        //        "true", //override
        //        "ColAValue", //ColumnA
        //        "ColBValue", //ColumnB
        //        "23", //IntColumn
        //        "19.99" //DecimalColumn
        //    };

        //    setupRepository.SaveLookupTableRow(row);
        //    var lookupTable = setupRepository.GetLookupTable(addedTable.ChangeId);
        //    var insertedRow = lookupTable.Rows.Last();

        //    Assert.IsTrue(insertedRow.KeyValues.First().LowValue == row.Values[7]);
        //    Assert.IsTrue(insertedRow.ColumnValues.First().Value.ToString() == row.Values[9]);

        //    setupRepository.DeleteLookupTableRow(string.Empty, addedTable.ChangeId, insertedRow.RowId);

        //    lookupTable = _repository.GetLookupTable(addedTable.ChangeId);

        //    Assert.IsTrue(!lookupTable.Rows.Any());
        //}

        //[TestMethod]
        //public void GetLookupTableKey()
        //{
        //    const string tableName = "PrivatePassengerOtherThanCollisionDeductibleRelativity";
        //    const string tableId = "6C84C211-032B-4531-A33B-A083B8A4406B";
        //    const string keyColumnName = "OtherThanCollisionDeductible";
        //    const string tableSchema = "ISO_AK";

        //    var key = _repository.GetLookupTableKey(string.Empty, tableId, keyColumnName);

        //    Assert.IsTrue(key != null);
        //    Assert.IsTrue(key.Name == keyColumnName);
        //    Assert.IsTrue(key.LookupType == LookupType.ExactMatch);
        //    Assert.IsTrue(key.TableSchema == tableSchema);
        //    Assert.IsTrue(key.TableName == tableName);
        //}

        //[TestMethod]
        //public void SaveLookupTableKey()
        //{
        //    const string tableId = "6C84C211-032B-4531-A33B-A083B8A4406B";
        //    const string keyColumnName = "OtherThanCollisionDeductible";

        //    var key = _repository.GetLookupTableKey(string.Empty, tableId, keyColumnName);
        //    Assert.IsTrue(key != null);
        //    var originalKeyLookupType = key.LookupType;

        //    key.LookupType = LookupType.GreaterThan;
        //    _repository.SaveLookupTableKey(key);

        //    var updatedKey = _repository.GetLookupTableKey(string.Empty, tableId, keyColumnName);
        //    Assert.IsTrue(updatedKey != null);
        //    Assert.IsTrue(originalKeyLookupType != updatedKey.LookupType);
        //}
        #endregion
    }
}
