﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.EntityClient;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;

[assembly: EdmSchemaAttribute()]

namespace More.Application.Entity
{
    #region Contexts
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class MoreLookupTableEntities : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new MoreLookupTableEntities object using the connection string found in the 'MoreLookupTableEntities' section of the application configuration file.
        /// </summary>
        public MoreLookupTableEntities() : base("name=MoreLookupTableEntities", "MoreLookupTableEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new MoreLookupTableEntities object.
        /// </summary>
        public MoreLookupTableEntities(string connectionString) : base(connectionString, "MoreLookupTableEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new MoreLookupTableEntities object.
        /// </summary>
        public MoreLookupTableEntities(EntityConnection connection) : base(connection, "MoreLookupTableEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        #endregion
    
        #region Partial Methods
    
        partial void OnContextCreated();
    
        #endregion
    
        #region Function Imports
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectResult<GetTableColumns_Result> GetTableColumns()
        {
            return base.ExecuteFunction<GetTableColumns_Result>("GetTableColumns");
        }
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectResult<GetNextFactorTableId_Result> GetNextFactorTableId()
        {
            return base.ExecuteFunction<GetNextFactorTableId_Result>("GetNextFactorTableId");
        }
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        /// <param name="effectiveDate">No Metadata Documentation available.</param>
        public ObjectResult<GetFactorTables_Result> GetFactorTables(Nullable<global::System.DateTime> effectiveDate)
        {
            ObjectParameter effectiveDateParameter;
            if (effectiveDate.HasValue)
            {
                effectiveDateParameter = new ObjectParameter("EffectiveDate", effectiveDate);
            }
            else
            {
                effectiveDateParameter = new ObjectParameter("EffectiveDate", typeof(global::System.DateTime));
            }
    
            return base.ExecuteFunction<GetFactorTables_Result>("GetFactorTables", effectiveDateParameter);
        }
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectResult<GetAllFactorTables_Result> GetAllFactorTables()
        {
            return base.ExecuteFunction<GetAllFactorTables_Result>("GetAllFactorTables");
        }

        #endregion
    }
    

    #endregion
    
    #region ComplexTypes
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmComplexTypeAttribute(NamespaceName="More.Application.Entity", Name="GetAllFactorTables_Result")]
    [DataContractAttribute(IsReference=true)]
    [Serializable()]
    public partial class GetAllFactorTables_Result : ComplexObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new GetAllFactorTables_Result object.
        /// </summary>
        /// <param name="tABLE_NAME">Initial value of the TABLE_NAME property.</param>
        public static GetAllFactorTables_Result CreateGetAllFactorTables_Result(global::System.String tABLE_NAME)
        {
            GetAllFactorTables_Result getAllFactorTables_Result = new GetAllFactorTables_Result();
            getAllFactorTables_Result.TABLE_NAME = tABLE_NAME;
            return getAllFactorTables_Result;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String TABLE_NAME
        {
            get
            {
                return _TABLE_NAME;
            }
            set
            {
                OnTABLE_NAMEChanging(value);
                ReportPropertyChanging("TABLE_NAME");
                _TABLE_NAME = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("TABLE_NAME");
                OnTABLE_NAMEChanged();
            }
        }
        private global::System.String _TABLE_NAME;
        partial void OnTABLE_NAMEChanging(global::System.String value);
        partial void OnTABLE_NAMEChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> Id
        {
            get
            {
                return _Id;
            }
            set
            {
                OnIdChanging(value);
                ReportPropertyChanging("Id");
                _Id = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Id");
                OnIdChanged();
            }
        }
        private Nullable<global::System.Int32> _Id;
        partial void OnIdChanging(Nullable<global::System.Int32> value);
        partial void OnIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Boolean> Active
        {
            get
            {
                return _Active;
            }
            set
            {
                OnActiveChanging(value);
                ReportPropertyChanging("Active");
                _Active = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Active");
                OnActiveChanged();
            }
        }
        private Nullable<global::System.Boolean> _Active;
        partial void OnActiveChanging(Nullable<global::System.Boolean> value);
        partial void OnActiveChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.DateTime> CreateDate
        {
            get
            {
                return _CreateDate;
            }
            set
            {
                OnCreateDateChanging(value);
                ReportPropertyChanging("CreateDate");
                _CreateDate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("CreateDate");
                OnCreateDateChanged();
            }
        }
        private Nullable<global::System.DateTime> _CreateDate;
        partial void OnCreateDateChanging(Nullable<global::System.DateTime> value);
        partial void OnCreateDateChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Guid> ChangeId
        {
            get
            {
                return _ChangeId;
            }
            set
            {
                OnChangeIdChanging(value);
                ReportPropertyChanging("ChangeId");
                _ChangeId = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ChangeId");
                OnChangeIdChanged();
            }
        }
        private Nullable<global::System.Guid> _ChangeId;
        partial void OnChangeIdChanging(Nullable<global::System.Guid> value);
        partial void OnChangeIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.DateTime> EffectiveDate
        {
            get
            {
                return _EffectiveDate;
            }
            set
            {
                OnEffectiveDateChanging(value);
                ReportPropertyChanging("EffectiveDate");
                _EffectiveDate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("EffectiveDate");
                OnEffectiveDateChanged();
            }
        }
        private Nullable<global::System.DateTime> _EffectiveDate;
        partial void OnEffectiveDateChanging(Nullable<global::System.DateTime> value);
        partial void OnEffectiveDateChanged();

        #endregion
    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmComplexTypeAttribute(NamespaceName="More.Application.Entity", Name="GetFactorTables_Result")]
    [DataContractAttribute(IsReference=true)]
    [Serializable()]
    public partial class GetFactorTables_Result : ComplexObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new GetFactorTables_Result object.
        /// </summary>
        /// <param name="tableName">Initial value of the TableName property.</param>
        public static GetFactorTables_Result CreateGetFactorTables_Result(global::System.String tableName)
        {
            GetFactorTables_Result getFactorTables_Result = new GetFactorTables_Result();
            getFactorTables_Result.TableName = tableName;
            return getFactorTables_Result;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> Id
        {
            get
            {
                return _Id;
            }
            set
            {
                OnIdChanging(value);
                ReportPropertyChanging("Id");
                _Id = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Id");
                OnIdChanged();
            }
        }
        private Nullable<global::System.Int32> _Id;
        partial void OnIdChanging(Nullable<global::System.Int32> value);
        partial void OnIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Boolean> Active
        {
            get
            {
                return _Active;
            }
            set
            {
                OnActiveChanging(value);
                ReportPropertyChanging("Active");
                _Active = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Active");
                OnActiveChanged();
            }
        }
        private Nullable<global::System.Boolean> _Active;
        partial void OnActiveChanging(Nullable<global::System.Boolean> value);
        partial void OnActiveChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.DateTime> CreateDate
        {
            get
            {
                return _CreateDate;
            }
            set
            {
                OnCreateDateChanging(value);
                ReportPropertyChanging("CreateDate");
                _CreateDate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("CreateDate");
                OnCreateDateChanged();
            }
        }
        private Nullable<global::System.DateTime> _CreateDate;
        partial void OnCreateDateChanging(Nullable<global::System.DateTime> value);
        partial void OnCreateDateChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Guid> ChangeId
        {
            get
            {
                return _ChangeId;
            }
            set
            {
                OnChangeIdChanging(value);
                ReportPropertyChanging("ChangeId");
                _ChangeId = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ChangeId");
                OnChangeIdChanged();
            }
        }
        private Nullable<global::System.Guid> _ChangeId;
        partial void OnChangeIdChanging(Nullable<global::System.Guid> value);
        partial void OnChangeIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.DateTime> EffectiveDate
        {
            get
            {
                return _EffectiveDate;
            }
            set
            {
                OnEffectiveDateChanging(value);
                ReportPropertyChanging("EffectiveDate");
                _EffectiveDate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("EffectiveDate");
                OnEffectiveDateChanged();
            }
        }
        private Nullable<global::System.DateTime> _EffectiveDate;
        partial void OnEffectiveDateChanging(Nullable<global::System.DateTime> value);
        partial void OnEffectiveDateChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                OnTableNameChanging(value);
                ReportPropertyChanging("TableName");
                _TableName = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("TableName");
                OnTableNameChanged();
            }
        }
        private global::System.String _TableName;
        partial void OnTableNameChanging(global::System.String value);
        partial void OnTableNameChanged();

        #endregion
    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmComplexTypeAttribute(NamespaceName="More.Application.Entity", Name="GetNextFactorTableId_Result")]
    [DataContractAttribute(IsReference=true)]
    [Serializable()]
    public partial class GetNextFactorTableId_Result : ComplexObject
    {
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> Column1
        {
            get
            {
                return _Column1;
            }
            set
            {
                OnColumn1Changing(value);
                ReportPropertyChanging("Column1");
                _Column1 = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Column1");
                OnColumn1Changed();
            }
        }
        private Nullable<global::System.Int32> _Column1;
        partial void OnColumn1Changing(Nullable<global::System.Int32> value);
        partial void OnColumn1Changed();

        #endregion
    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmComplexTypeAttribute(NamespaceName="More.Application.Entity", Name="GetTableColumns_Result")]
    [DataContractAttribute(IsReference=true)]
    [Serializable()]
    public partial class GetTableColumns_Result : ComplexObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new GetTableColumns_Result object.
        /// </summary>
        /// <param name="tableName">Initial value of the TableName property.</param>
        public static GetTableColumns_Result CreateGetTableColumns_Result(global::System.String tableName)
        {
            GetTableColumns_Result getTableColumns_Result = new GetTableColumns_Result();
            getTableColumns_Result.TableName = tableName;
            return getTableColumns_Result;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                OnTableNameChanging(value);
                ReportPropertyChanging("TableName");
                _TableName = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("TableName");
                OnTableNameChanged();
            }
        }
        private global::System.String _TableName;
        partial void OnTableNameChanging(global::System.String value);
        partial void OnTableNameChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String ColumnName
        {
            get
            {
                return _ColumnName;
            }
            set
            {
                OnColumnNameChanging(value);
                ReportPropertyChanging("ColumnName");
                _ColumnName = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("ColumnName");
                OnColumnNameChanged();
            }
        }
        private global::System.String _ColumnName;
        partial void OnColumnNameChanging(global::System.String value);
        partial void OnColumnNameChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Boolean> IsKey
        {
            get
            {
                return _IsKey;
            }
            set
            {
                OnIsKeyChanging(value);
                ReportPropertyChanging("IsKey");
                _IsKey = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("IsKey");
                OnIsKeyChanged();
            }
        }
        private Nullable<global::System.Boolean> _IsKey;
        partial void OnIsKeyChanging(Nullable<global::System.Boolean> value);
        partial void OnIsKeyChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String LookupType
        {
            get
            {
                return _LookupType;
            }
            set
            {
                OnLookupTypeChanging(value);
                ReportPropertyChanging("LookupType");
                _LookupType = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("LookupType");
                OnLookupTypeChanged();
            }
        }
        private global::System.String _LookupType;
        partial void OnLookupTypeChanging(global::System.String value);
        partial void OnLookupTypeChanged();

        #endregion
    }

    #endregion
    
}
