






#pragma warning disable 1591
namespace ConvenienceStore.Data
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="ConvenienceStoreDB")]
	public partial class CSDbContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertRole(Role instance);
    partial void UpdateRole(Role instance);
    partial void DeleteRole(Role instance);
    partial void InsertUser(User instance);
    partial void UpdateUser(User instance);
    partial void DeleteUser(User instance);
    partial void InsertCategory(Category instance);
    partial void UpdateCategory(Category instance);
    partial void DeleteCategory(Category instance);
    partial void InsertProduct(Product instance);
    partial void UpdateProduct(Product instance);
    partial void DeleteProduct(Product instance);
    partial void InsertSupplier(Supplier instance);
    partial void UpdateSupplier(Supplier instance);
    partial void DeleteSupplier(Supplier instance);
    partial void InsertCustomer(Customer instance);
    partial void UpdateCustomer(Customer instance);
    partial void DeleteCustomer(Customer instance);
    partial void InsertPurchaseReceipt(PurchaseReceipt instance);
    partial void UpdatePurchaseReceipt(PurchaseReceipt instance);
    partial void DeletePurchaseReceipt(PurchaseReceipt instance);
    partial void InsertPurchaseReceiptLine(PurchaseReceiptLine instance);
    partial void UpdatePurchaseReceiptLine(PurchaseReceiptLine instance);
    partial void DeletePurchaseReceiptLine(PurchaseReceiptLine instance);
    partial void InsertSaleInvoice(SaleInvoice instance);
    partial void UpdateSaleInvoice(SaleInvoice instance);
    partial void DeleteSaleInvoice(SaleInvoice instance);
    partial void InsertSaleInvoiceLine(SaleInvoiceLine instance);
    partial void UpdateSaleInvoiceLine(SaleInvoiceLine instance);
    partial void DeleteSaleInvoiceLine(SaleInvoiceLine instance);
    partial void InsertStockMovement(StockMovement instance);
    partial void UpdateStockMovement(StockMovement instance);
    partial void DeleteStockMovement(StockMovement instance);
    #endregion
		
		public CSDbContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public CSDbContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public CSDbContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public CSDbContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}

		
		public System.Data.Linq.Table<Role> Roles
		{
			get
			{
				return this.GetTable<Role>();
			}
		}
		
		public System.Data.Linq.Table<User> Users
		{
			get
			{
				return this.GetTable<User>();
			}
		}
		
		public System.Data.Linq.Table<Category> Categories
		{
			get
			{
				return this.GetTable<Category>();
			}
		}
		
		public System.Data.Linq.Table<Product> Products
		{
			get
			{
				return this.GetTable<Product>();
			}
		}
		
		public System.Data.Linq.Table<Supplier> Suppliers
		{
			get
			{
				return this.GetTable<Supplier>();
			}
		}
		
		public System.Data.Linq.Table<Customer> Customers
		{
			get
			{
				return this.GetTable<Customer>();
			}
		}
		
		public System.Data.Linq.Table<PurchaseReceipt> PurchaseReceipts
		{
			get
			{
				return this.GetTable<PurchaseReceipt>();
			}
		}
		
		public System.Data.Linq.Table<PurchaseReceiptLine> PurchaseReceiptLines
		{
			get
			{
				return this.GetTable<PurchaseReceiptLine>();
			}
		}
		
		public System.Data.Linq.Table<SaleInvoice> SaleInvoices
		{
			get
			{
				return this.GetTable<SaleInvoice>();
			}
		}
		
		public System.Data.Linq.Table<SaleInvoiceLine> SaleInvoiceLines
		{
			get
			{
				return this.GetTable<SaleInvoiceLine>();
			}
		}
		
		public System.Data.Linq.Table<StockMovement> StockMovements
		{
			get
			{
				return this.GetTable<StockMovement>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Roles")]
	public partial class Role : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

		private int _Id;
		private string _Name;
		private EntitySet<User> _Users;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    #endregion
		
		public Role()
		{
			this._Users = new EntitySet<User>(new Action<User>(this.attach_Users), new Action<User>(this.detach_Users));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Role_User", Storage="_Users", ThisKey="Id", OtherKey="RoleId")]
		public EntitySet<User> Users
		{
			get
			{
				return this._Users;
			}
			set
			{
				this._Users.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Users(User entity)
		{
			this.SendPropertyChanging();
			entity.Role = this;
		}
		
		private void detach_Users(User entity)
		{
			this.SendPropertyChanging();
			entity.Role = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Users")]
	public partial class User : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

		private int _Id;
		private string _Username;
		private string _PasswordHash;
		private string _FullName;
		private bool _IsActive;
		private int _RoleId;
		private EntityRef<Role> _Role;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnUsernameChanging(string value);
    partial void OnUsernameChanged();
    partial void OnPasswordHashChanging(string value);
    partial void OnPasswordHashChanged();
    partial void OnFullNameChanging(string value);
    partial void OnFullNameChanged();
    partial void OnIsActiveChanging(bool value);
    partial void OnIsActiveChanged();
    partial void OnRoleIdChanging(int value);
    partial void OnRoleIdChanged();
    #endregion
		
		public User()
		{
			this._Role = default(EntityRef<Role>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Username", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Username
		{
			get
			{
				return this._Username;
			}
			set
			{
				if ((this._Username != value))
				{
					this.OnUsernameChanging(value);
					this.SendPropertyChanging();
					this._Username = value;
					this.SendPropertyChanged("Username");
					this.OnUsernameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PasswordHash", DbType="NVarChar(200) NOT NULL", CanBeNull=false)]
		public string PasswordHash
		{
			get
			{
				return this._PasswordHash;
			}
			set
			{
				if ((this._PasswordHash != value))
				{
					this.OnPasswordHashChanging(value);
					this.SendPropertyChanging();
					this._PasswordHash = value;
					this.SendPropertyChanged("PasswordHash");
					this.OnPasswordHashChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FullName", DbType="NVarChar(100)")]
		public string FullName
		{
			get
			{
				return this._FullName;
			}
			set
			{
				if ((this._FullName != value))
				{
					this.OnFullNameChanging(value);
					this.SendPropertyChanging();
					this._FullName = value;
					this.SendPropertyChanged("FullName");
					this.OnFullNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsActive", DbType="Bit NOT NULL")]
		public bool IsActive
		{
			get
			{
				return this._IsActive;
			}
			set
			{
				if ((this._IsActive != value))
				{
					this.OnIsActiveChanging(value);
					this.SendPropertyChanging();
					this._IsActive = value;
					this.SendPropertyChanged("IsActive");
					this.OnIsActiveChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RoleId", DbType="Int NOT NULL")]
		public int RoleId
		{
			get
			{
				return this._RoleId;
			}
			set
			{
				if ((this._RoleId != value))
				{
					if (this._Role.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnRoleIdChanging(value);
					this.SendPropertyChanging();
					this._RoleId = value;
					this.SendPropertyChanged("RoleId");
					this.OnRoleIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Role_User", Storage="_Role", ThisKey="RoleId", OtherKey="Id", IsForeignKey=true)]
		public Role Role
		{
			get
			{
				return this._Role.Entity;
			}
			set
			{
				Role previousValue = this._Role.Entity;
				if (((previousValue != value) 
							|| (this._Role.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Role.Entity = null;
						previousValue.Users.Remove(this);
					}
					this._Role.Entity = value;
					if ((value != null))
					{
						value.Users.Add(this);
						this._RoleId = value.Id;
					}
					else
					{
						this._RoleId = default(int);
					}
					this.SendPropertyChanged("Role");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Categories")]
	public partial class Category : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

		private int _Id;
		private string _Code;
		private string _Name;
		private EntitySet<Product> _Products;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnCodeChanging(string value);
    partial void OnCodeChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    #endregion
		
		public Category()
		{
			this._Products = new EntitySet<Product>(new Action<Product>(this.attach_Products), new Action<Product>(this.detach_Products));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Code", DbType="NVarChar(30) NOT NULL", CanBeNull=false)]
		public string Code
		{
			get
			{
				return this._Code;
			}
			set
			{
				if ((this._Code != value))
				{
					this.OnCodeChanging(value);
					this.SendPropertyChanging();
					this._Code = value;
					this.SendPropertyChanged("Code");
					this.OnCodeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Category_Product", Storage="_Products", ThisKey="Id", OtherKey="CategoryId")]
		public EntitySet<Product> Products
		{
			get
			{
				return this._Products;
			}
			set
			{
				this._Products.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Products(Product entity)
		{
			this.SendPropertyChanging();
			entity.Category = this;
		}
		
		private void detach_Products(Product entity)
		{
			this.SendPropertyChanging();
			entity.Category = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Products")]
	public partial class Product : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

		private int _Id;
		private string _Name;
		private string _Barcode;
		private decimal _Price;
		private int _Stock;
		private int _CategoryId;
		private EntityRef<Category> _Category;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnBarcodeChanging(string value);
    partial void OnBarcodeChanged();
    partial void OnPriceChanging(decimal value);
    partial void OnPriceChanged();
    partial void OnStockChanging(int value);
    partial void OnStockChanged();
    partial void OnCategoryIdChanging(int value);
    partial void OnCategoryIdChanged();
    #endregion
		
		public Product()
		{
			this._Category = default(EntityRef<Category>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Barcode", DbType="NVarChar(50)")]
		public string Barcode
		{
			get
			{
				return this._Barcode;
			}
			set
			{
				if ((this._Barcode != value))
				{
					this.OnBarcodeChanging(value);
					this.SendPropertyChanging();
					this._Barcode = value;
					this.SendPropertyChanged("Barcode");
					this.OnBarcodeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Price", DbType="Decimal(18,2) NOT NULL")]
		public decimal Price
		{
			get
			{
				return this._Price;
			}
			set
			{
				if ((this._Price != value))
				{
					this.OnPriceChanging(value);
					this.SendPropertyChanging();
					this._Price = value;
					this.SendPropertyChanged("Price");
					this.OnPriceChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Stock", DbType="Int NOT NULL")]
		public int Stock
		{
			get
			{
				return this._Stock;
			}
			set
			{
				if ((this._Stock != value))
				{
					this.OnStockChanging(value);
					this.SendPropertyChanging();
					this._Stock = value;
					this.SendPropertyChanged("Stock");
					this.OnStockChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CategoryId", DbType="Int NOT NULL")]
		public int CategoryId
		{
			get
			{
				return this._CategoryId;
			}
			set
			{
				if ((this._CategoryId != value))
				{
					if (this._Category.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnCategoryIdChanging(value);
					this.SendPropertyChanging();
					this._CategoryId = value;
					this.SendPropertyChanged("CategoryId");
					this.OnCategoryIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Category_Product", Storage="_Category", ThisKey="CategoryId", OtherKey="Id", IsForeignKey=true)]
		public Category Category
		{
			get
			{
				return this._Category.Entity;
			}
			set
			{
				Category previousValue = this._Category.Entity;
				if (((previousValue != value) 
							|| (this._Category.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Category.Entity = null;
						previousValue.Products.Remove(this);
					}
					this._Category.Entity = value;
					if ((value != null))
					{
						value.Products.Add(this);
						this._CategoryId = value.Id;
					}
					else
					{
						this._CategoryId = default(int);
					}
					this.SendPropertyChanged("Category");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Suppliers")]
	public partial class Supplier : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

		private int _Id;
		private string _Name;
		private string _Phone;
		private bool _IsActive;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnPhoneChanging(string value);
    partial void OnPhoneChanged();
    partial void OnIsActiveChanging(bool value);
    partial void OnIsActiveChanged();
    #endregion
		
		public Supplier()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Phone", DbType="NVarChar(30)")]
		public string Phone
		{
			get
			{
				return this._Phone;
			}
			set
			{
				if ((this._Phone != value))
				{
					this.OnPhoneChanging(value);
					this.SendPropertyChanging();
					this._Phone = value;
					this.SendPropertyChanged("Phone");
					this.OnPhoneChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsActive", DbType="Bit NOT NULL")]
		public bool IsActive
		{
			get
			{
				return this._IsActive;
			}
			set
			{
				if ((this._IsActive != value))
				{
					this.OnIsActiveChanging(value);
					this.SendPropertyChanging();
					this._IsActive = value;
					this.SendPropertyChanged("IsActive");
					this.OnIsActiveChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Customers")]
	public partial class Customer : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

		private int _Id;
		private string _Name;
		private string _Phone;
		private bool _IsVip;
		private bool _IsActive;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnPhoneChanging(string value);
    partial void OnPhoneChanged();
    partial void OnIsVipChanging(bool value);
    partial void OnIsVipChanged();
    partial void OnIsActiveChanging(bool value);
    partial void OnIsActiveChanged();
    #endregion
		
		public Customer()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Phone", DbType="NVarChar(30)")]
		public string Phone
		{
			get
			{
				return this._Phone;
			}
			set
			{
				if ((this._Phone != value))
				{
					this.OnPhoneChanging(value);
					this.SendPropertyChanging();
					this._Phone = value;
					this.SendPropertyChanged("Phone");
					this.OnPhoneChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsVip", DbType="Bit NOT NULL")]
		public bool IsVip
		{
			get
			{
				return this._IsVip;
			}
			set
			{
				if ((this._IsVip != value))
				{
					this.OnIsVipChanging(value);
					this.SendPropertyChanging();
					this._IsVip = value;
					this.SendPropertyChanged("IsVip");
					this.OnIsVipChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsActive", DbType="Bit NOT NULL")]
		public bool IsActive
		{
			get
			{
				return this._IsActive;
			}
			set
			{
				if ((this._IsActive != value))
				{
					this.OnIsActiveChanging(value);
					this.SendPropertyChanging();
					this._IsActive = value;
					this.SendPropertyChanged("IsActive");
					this.OnIsActiveChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.PurchaseReceipts")]
	public partial class PurchaseReceipt : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

		private int _Id;
		private System.Guid _OperationId;
		private string _Number;
		private System.DateTime _CreatedAt;
		private int _SupplierId;
		private int _CreatedById;
		private string _SupplierName;
		private string _Note;
		private decimal _Total;
		private EntitySet<PurchaseReceiptLine> _Lines;
		private EntityRef<Supplier> _Supplier;
		private EntityRef<User> _CreatedBy;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnOperationIdChanging(System.Guid value);
    partial void OnOperationIdChanged();
    partial void OnNumberChanging(string value);
    partial void OnNumberChanged();
    partial void OnCreatedAtChanging(System.DateTime value);
    partial void OnCreatedAtChanged();
    partial void OnSupplierIdChanging(int value);
    partial void OnSupplierIdChanged();
    partial void OnCreatedByIdChanging(int value);
    partial void OnCreatedByIdChanged();
    partial void OnSupplierNameChanging(string value);
    partial void OnSupplierNameChanged();
    partial void OnNoteChanging(string value);
    partial void OnNoteChanged();
    partial void OnTotalChanging(decimal value);
    partial void OnTotalChanged();
    #endregion
		
		public PurchaseReceipt()
		{
			this._Lines = new EntitySet<PurchaseReceiptLine>(new Action<PurchaseReceiptLine>(this.attach_Lines), new Action<PurchaseReceiptLine>(this.detach_Lines));
			this._Supplier = default(EntityRef<Supplier>);
			this._CreatedBy = default(EntityRef<User>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_OperationId", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid OperationId
		{
			get
			{
				return this._OperationId;
			}
			set
			{
				if ((this._OperationId != value))
				{
					this.OnOperationIdChanging(value);
					this.SendPropertyChanging();
					this._OperationId = value;
					this.SendPropertyChanged("OperationId");
					this.OnOperationIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Number", DbType="NVarChar(40) NOT NULL", CanBeNull=false)]
		public string Number
		{
			get
			{
				return this._Number;
			}
			set
			{
				if ((this._Number != value))
				{
					this.OnNumberChanging(value);
					this.SendPropertyChanging();
					this._Number = value;
					this.SendPropertyChanged("Number");
					this.OnNumberChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CreatedAt", DbType="DateTime2 NOT NULL")]
		public System.DateTime CreatedAt
		{
			get
			{
				return this._CreatedAt;
			}
			set
			{
				if ((this._CreatedAt != value))
				{
					this.OnCreatedAtChanging(value);
					this.SendPropertyChanging();
					this._CreatedAt = value;
					this.SendPropertyChanged("CreatedAt");
					this.OnCreatedAtChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SupplierId", DbType="Int NOT NULL")]
		public int SupplierId
		{
			get
			{
				return this._SupplierId;
			}
			set
			{
				if ((this._SupplierId != value))
				{
					if (this._Supplier.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnSupplierIdChanging(value);
					this.SendPropertyChanging();
					this._SupplierId = value;
					this.SendPropertyChanged("SupplierId");
					this.OnSupplierIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CreatedById", DbType="Int NOT NULL")]
		public int CreatedById
		{
			get
			{
				return this._CreatedById;
			}
			set
			{
				if ((this._CreatedById != value))
				{
					if (this._CreatedBy.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnCreatedByIdChanging(value);
					this.SendPropertyChanging();
					this._CreatedById = value;
					this.SendPropertyChanged("CreatedById");
					this.OnCreatedByIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SupplierName", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string SupplierName
		{
			get
			{
				return this._SupplierName;
			}
			set
			{
				if ((this._SupplierName != value))
				{
					this.OnSupplierNameChanging(value);
					this.SendPropertyChanging();
					this._SupplierName = value;
					this.SendPropertyChanged("SupplierName");
					this.OnSupplierNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Note", DbType="NVarChar(300)")]
		public string Note
		{
			get
			{
				return this._Note;
			}
			set
			{
				if ((this._Note != value))
				{
					this.OnNoteChanging(value);
					this.SendPropertyChanging();
					this._Note = value;
					this.SendPropertyChanged("Note");
					this.OnNoteChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Total", DbType="Decimal(18,2) NOT NULL")]
		public decimal Total
		{
			get
			{
				return this._Total;
			}
			set
			{
				if ((this._Total != value))
				{
					this.OnTotalChanging(value);
					this.SendPropertyChanging();
					this._Total = value;
					this.SendPropertyChanged("Total");
					this.OnTotalChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Supplier_PurchaseReceipt", Storage="_Supplier", ThisKey="SupplierId", OtherKey="Id", IsForeignKey=true)]
		public Supplier Supplier
		{
			get
			{
				return this._Supplier.Entity;
			}
			set
			{
				Supplier previousValue = this._Supplier.Entity;
				if (((previousValue != value) 
							|| (this._Supplier.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Supplier.Entity = null;
					}
					this._Supplier.Entity = value;
					if ((value != null))
					{
						this._SupplierId = value.Id;
					}
					else
					{
						this._SupplierId = default(int);
					}
					this.SendPropertyChanged("Supplier");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_PurchaseReceipt", Storage="_CreatedBy", ThisKey="CreatedById", OtherKey="Id", IsForeignKey=true)]
		public User CreatedBy
		{
			get
			{
				return this._CreatedBy.Entity;
			}
			set
			{
				User previousValue = this._CreatedBy.Entity;
				if (((previousValue != value) 
							|| (this._CreatedBy.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._CreatedBy.Entity = null;
					}
					this._CreatedBy.Entity = value;
					if ((value != null))
					{
						this._CreatedById = value.Id;
					}
					else
					{
						this._CreatedById = default(int);
					}
					this.SendPropertyChanged("CreatedBy");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="PurchaseReceipt_PurchaseReceiptLine", Storage="_Lines", ThisKey="Id", OtherKey="PurchaseReceiptId")]
		public EntitySet<PurchaseReceiptLine> Lines
		{
			get
			{
				return this._Lines;
			}
			set
			{
				this._Lines.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Lines(PurchaseReceiptLine entity)
		{
			this.SendPropertyChanging();
			entity.PurchaseReceipt = this;
		}
		
		private void detach_Lines(PurchaseReceiptLine entity)
		{
			this.SendPropertyChanging();
			entity.PurchaseReceipt = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.PurchaseReceiptLines")]
	public partial class PurchaseReceiptLine : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

		private int _Id;
		private int _PurchaseReceiptId;
		private int _ProductId;
		private string _ProductName;
		private int _Quantity;
		private decimal _UnitCost;
		private decimal _LineTotal;
		private EntityRef<PurchaseReceipt> _PurchaseReceipt;
		private EntityRef<Product> _Product;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnPurchaseReceiptIdChanging(int value);
    partial void OnPurchaseReceiptIdChanged();
    partial void OnProductIdChanging(int value);
    partial void OnProductIdChanged();
    partial void OnProductNameChanging(string value);
    partial void OnProductNameChanged();
    partial void OnQuantityChanging(int value);
    partial void OnQuantityChanged();
    partial void OnUnitCostChanging(decimal value);
    partial void OnUnitCostChanged();
    partial void OnLineTotalChanging(decimal value);
    partial void OnLineTotalChanged();
    #endregion
		
		public PurchaseReceiptLine()
		{
			this._PurchaseReceipt = default(EntityRef<PurchaseReceipt>);
			this._Product = default(EntityRef<Product>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PurchaseReceiptId", DbType="Int NOT NULL")]
		public int PurchaseReceiptId
		{
			get
			{
				return this._PurchaseReceiptId;
			}
			set
			{
				if ((this._PurchaseReceiptId != value))
				{
					if (this._PurchaseReceipt.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnPurchaseReceiptIdChanging(value);
					this.SendPropertyChanging();
					this._PurchaseReceiptId = value;
					this.SendPropertyChanged("PurchaseReceiptId");
					this.OnPurchaseReceiptIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ProductId", DbType="Int NOT NULL")]
		public int ProductId
		{
			get
			{
				return this._ProductId;
			}
			set
			{
				if ((this._ProductId != value))
				{
					if (this._Product.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnProductIdChanging(value);
					this.SendPropertyChanging();
					this._ProductId = value;
					this.SendPropertyChanged("ProductId");
					this.OnProductIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ProductName", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string ProductName
		{
			get
			{
				return this._ProductName;
			}
			set
			{
				if ((this._ProductName != value))
				{
					this.OnProductNameChanging(value);
					this.SendPropertyChanging();
					this._ProductName = value;
					this.SendPropertyChanged("ProductName");
					this.OnProductNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Quantity", DbType="Int NOT NULL")]
		public int Quantity
		{
			get
			{
				return this._Quantity;
			}
			set
			{
				if ((this._Quantity != value))
				{
					this.OnQuantityChanging(value);
					this.SendPropertyChanging();
					this._Quantity = value;
					this.SendPropertyChanged("Quantity");
					this.OnQuantityChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UnitCost", DbType="Decimal(18,2) NOT NULL")]
		public decimal UnitCost
		{
			get
			{
				return this._UnitCost;
			}
			set
			{
				if ((this._UnitCost != value))
				{
					this.OnUnitCostChanging(value);
					this.SendPropertyChanging();
					this._UnitCost = value;
					this.SendPropertyChanged("UnitCost");
					this.OnUnitCostChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LineTotal", DbType="Decimal(18,2) NOT NULL")]
		public decimal LineTotal
		{
			get
			{
				return this._LineTotal;
			}
			set
			{
				if ((this._LineTotal != value))
				{
					this.OnLineTotalChanging(value);
					this.SendPropertyChanging();
					this._LineTotal = value;
					this.SendPropertyChanged("LineTotal");
					this.OnLineTotalChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="PurchaseReceipt_PurchaseReceiptLine", Storage="_PurchaseReceipt", ThisKey="PurchaseReceiptId", OtherKey="Id", IsForeignKey=true)]
		public PurchaseReceipt PurchaseReceipt
		{
			get
			{
				return this._PurchaseReceipt.Entity;
			}
			set
			{
				PurchaseReceipt previousValue = this._PurchaseReceipt.Entity;
				if (((previousValue != value) 
							|| (this._PurchaseReceipt.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._PurchaseReceipt.Entity = null;
						previousValue.Lines.Remove(this);
					}
					this._PurchaseReceipt.Entity = value;
					if ((value != null))
					{
						value.Lines.Add(this);
						this._PurchaseReceiptId = value.Id;
					}
					else
					{
						this._PurchaseReceiptId = default(int);
					}
					this.SendPropertyChanged("PurchaseReceipt");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Product_PurchaseReceiptLine", Storage="_Product", ThisKey="ProductId", OtherKey="Id", IsForeignKey=true)]
		public Product Product
		{
			get
			{
				return this._Product.Entity;
			}
			set
			{
				Product previousValue = this._Product.Entity;
				if (((previousValue != value) 
							|| (this._Product.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Product.Entity = null;
					}
					this._Product.Entity = value;
					if ((value != null))
					{
						this._ProductId = value.Id;
					}
					else
					{
						this._ProductId = default(int);
					}
					this.SendPropertyChanged("Product");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.SaleInvoices")]
	public partial class SaleInvoice : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

		private int _Id;
		private System.Guid _OperationId;
		private string _Number;
		private System.DateTime _CreatedAt;
		private System.Nullable<int> _CustomerId;
		private string _CustomerName;
		private int _CreatedById;
		private decimal _Subtotal;
		private decimal _DiscountPercent;
		private decimal _DiscountAmount;
		private decimal _Total;
		private decimal _CashReceived;
		private decimal _Change;
		
		private string _PromotionCode;
		
		private decimal _PromotionAmount;
		private bool _IsCancelled;
		
		private System.Nullable<System.DateTime> _CancelledAt;
		
		private System.Nullable<int> _CancelledById;
		
		private string _CancelReason;
		
		private EntitySet<SaleInvoiceLine> _Lines;
		private EntityRef<Customer> _Customer;
		private EntityRef<User> _CreatedBy;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnOperationIdChanging(System.Guid value);
    partial void OnOperationIdChanged();
    partial void OnNumberChanging(string value);
    partial void OnNumberChanged();
    partial void OnCreatedAtChanging(System.DateTime value);
    partial void OnCreatedAtChanged();
    partial void OnCustomerIdChanging(System.Nullable<int> value);
    partial void OnCustomerIdChanged();
    partial void OnCustomerNameChanging(string value);
    partial void OnCustomerNameChanged();
    partial void OnCreatedByIdChanging(int value);
    partial void OnCreatedByIdChanged();
    partial void OnSubtotalChanging(decimal value);
    partial void OnSubtotalChanged();
    partial void OnDiscountPercentChanging(decimal value);
    partial void OnDiscountPercentChanged();
    partial void OnDiscountAmountChanging(decimal value);
    partial void OnDiscountAmountChanged();
    partial void OnTotalChanging(decimal value);
    partial void OnTotalChanged();
    partial void OnCashReceivedChanging(decimal value);
    partial void OnCashReceivedChanged();
    partial void OnChangeChanging(decimal value);
    partial void OnChangeChanged();
    partial void OnPromotionCodeChanging(string value);
    partial void OnPromotionCodeChanged();
    partial void OnPromotionAmountChanging(decimal value);
    partial void OnPromotionAmountChanged();
    partial void OnIsCancelledChanging(bool value);
    partial void OnIsCancelledChanged();
    partial void OnCancelledAtChanging(System.Nullable<System.DateTime> value);
    partial void OnCancelledAtChanged();
    partial void OnCancelledByIdChanging(System.Nullable<int> value);
    partial void OnCancelledByIdChanged();
    partial void OnCancelReasonChanging(string value);
    partial void OnCancelReasonChanged();
    #endregion
		
		public SaleInvoice()
		{
			this._Lines = new EntitySet<SaleInvoiceLine>(new Action<SaleInvoiceLine>(this.attach_Lines), new Action<SaleInvoiceLine>(this.detach_Lines));
			this._Customer = default(EntityRef<Customer>);
			this._CreatedBy = default(EntityRef<User>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_OperationId", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid OperationId
		{
			get
			{
				return this._OperationId;
			}
			set
			{
				if ((this._OperationId != value))
				{
					this.OnOperationIdChanging(value);
					this.SendPropertyChanging();
					this._OperationId = value;
					this.SendPropertyChanged("OperationId");
					this.OnOperationIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Number", DbType="NVarChar(40) NOT NULL", CanBeNull=false)]
		public string Number
		{
			get
			{
				return this._Number;
			}
			set
			{
				if ((this._Number != value))
				{
					this.OnNumberChanging(value);
					this.SendPropertyChanging();
					this._Number = value;
					this.SendPropertyChanged("Number");
					this.OnNumberChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CreatedAt", DbType="DateTime2 NOT NULL")]
		public System.DateTime CreatedAt
		{
			get
			{
				return this._CreatedAt;
			}
			set
			{
				if ((this._CreatedAt != value))
				{
					this.OnCreatedAtChanging(value);
					this.SendPropertyChanging();
					this._CreatedAt = value;
					this.SendPropertyChanged("CreatedAt");
					this.OnCreatedAtChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CustomerId", DbType="Int")]
		public System.Nullable<int> CustomerId
		{
			get
			{
				return this._CustomerId;
			}
			set
			{
				if ((this._CustomerId != value))
				{
					if (this._Customer.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnCustomerIdChanging(value);
					this.SendPropertyChanging();
					this._CustomerId = value;
					this.SendPropertyChanged("CustomerId");
					this.OnCustomerIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CustomerName", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string CustomerName
		{
			get
			{
				return this._CustomerName;
			}
			set
			{
				if ((this._CustomerName != value))
				{
					this.OnCustomerNameChanging(value);
					this.SendPropertyChanging();
					this._CustomerName = value;
					this.SendPropertyChanged("CustomerName");
					this.OnCustomerNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CreatedById", DbType="Int NOT NULL")]
		public int CreatedById
		{
			get
			{
				return this._CreatedById;
			}
			set
			{
				if ((this._CreatedById != value))
				{
					if (this._CreatedBy.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnCreatedByIdChanging(value);
					this.SendPropertyChanging();
					this._CreatedById = value;
					this.SendPropertyChanged("CreatedById");
					this.OnCreatedByIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Subtotal", DbType="Decimal(18,2) NOT NULL")]
		public decimal Subtotal
		{
			get
			{
				return this._Subtotal;
			}
			set
			{
				if ((this._Subtotal != value))
				{
					this.OnSubtotalChanging(value);
					this.SendPropertyChanging();
					this._Subtotal = value;
					this.SendPropertyChanged("Subtotal");
					this.OnSubtotalChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DiscountPercent", DbType="Decimal(18,2) NOT NULL")]
		public decimal DiscountPercent
		{
			get
			{
				return this._DiscountPercent;
			}
			set
			{
				if ((this._DiscountPercent != value))
				{
					this.OnDiscountPercentChanging(value);
					this.SendPropertyChanging();
					this._DiscountPercent = value;
					this.SendPropertyChanged("DiscountPercent");
					this.OnDiscountPercentChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DiscountAmount", DbType="Decimal(18,2) NOT NULL")]
		public decimal DiscountAmount
		{
			get
			{
				return this._DiscountAmount;
			}
			set
			{
				if ((this._DiscountAmount != value))
				{
					this.OnDiscountAmountChanging(value);
					this.SendPropertyChanging();
					this._DiscountAmount = value;
					this.SendPropertyChanged("DiscountAmount");
					this.OnDiscountAmountChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Total", DbType="Decimal(18,2) NOT NULL")]
		public decimal Total
		{
			get
			{
				return this._Total;
			}
			set
			{
				if ((this._Total != value))
				{
					this.OnTotalChanging(value);
					this.SendPropertyChanging();
					this._Total = value;
					this.SendPropertyChanged("Total");
					this.OnTotalChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CashReceived", DbType="Decimal(18,2) NOT NULL")]
		public decimal CashReceived
		{
			get
			{
				return this._CashReceived;
			}
			set
			{
				if ((this._CashReceived != value))
				{
					this.OnCashReceivedChanging(value);
					this.SendPropertyChanging();
					this._CashReceived = value;
					this.SendPropertyChanged("CashReceived");
					this.OnCashReceivedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Change", DbType="Decimal(18,2) NOT NULL")]
		public decimal Change
		{
			get
			{
				return this._Change;
			}
			set
			{
				if ((this._Change != value))
				{
					this.OnChangeChanging(value);
					this.SendPropertyChanging();
					this._Change = value;
					this.SendPropertyChanged("Change");
					this.OnChangeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PromotionCode", DbType="NVarChar(30)")]
		public string PromotionCode
		{
			get
			{
				return this._PromotionCode;
			}
			set
			{
				if ((this._PromotionCode != value))
				{
					this.OnPromotionCodeChanging(value);
					this.SendPropertyChanging();
					this._PromotionCode = value;
					this.SendPropertyChanged("PromotionCode");
					this.OnPromotionCodeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PromotionAmount", DbType="Decimal(18,2) NOT NULL")]
		public decimal PromotionAmount
		{
			get
			{
				return this._PromotionAmount;
			}
			set
			{
				if ((this._PromotionAmount != value))
				{
					this.OnPromotionAmountChanging(value);
					this.SendPropertyChanging();
					this._PromotionAmount = value;
					this.SendPropertyChanged("PromotionAmount");
					this.OnPromotionAmountChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsCancelled", DbType="Bit NOT NULL")]
		public bool IsCancelled
		{
			get
			{
				return this._IsCancelled;
			}
			set
			{
				if ((this._IsCancelled != value))
				{
					this.OnIsCancelledChanging(value);
					this.SendPropertyChanging();
					this._IsCancelled = value;
					this.SendPropertyChanged("IsCancelled");
					this.OnIsCancelledChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CancelledAt", DbType="DateTime2")]
		public System.Nullable<System.DateTime> CancelledAt
		{
			get
			{
				return this._CancelledAt;
			}
			set
			{
				if ((this._CancelledAt != value))
				{
					this.OnCancelledAtChanging(value);
					this.SendPropertyChanging();
					this._CancelledAt = value;
					this.SendPropertyChanged("CancelledAt");
					this.OnCancelledAtChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CancelledById", DbType="Int")]
		public System.Nullable<int> CancelledById
		{
			get
			{
				return this._CancelledById;
			}
			set
			{
				if ((this._CancelledById != value))
				{
					this.OnCancelledByIdChanging(value);
					this.SendPropertyChanging();
					this._CancelledById = value;
					this.SendPropertyChanged("CancelledById");
					this.OnCancelledByIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CancelReason", DbType="NVarChar(200)")]
		public string CancelReason
		{
			get
			{
				return this._CancelReason;
			}
			set
			{
				if ((this._CancelReason != value))
				{
					this.OnCancelReasonChanging(value);
					this.SendPropertyChanging();
					this._CancelReason = value;
					this.SendPropertyChanged("CancelReason");
					this.OnCancelReasonChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Customer_SaleInvoice", Storage="_Customer", ThisKey="CustomerId", OtherKey="Id", IsForeignKey=true)]
		public Customer Customer
		{
			get
			{
				return this._Customer.Entity;
			}
			set
			{
				Customer previousValue = this._Customer.Entity;
				if (((previousValue != value) 
							|| (this._Customer.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Customer.Entity = null;
					}
					this._Customer.Entity = value;
					if ((value != null))
					{
						this._CustomerId = value.Id;
					}
					else
					{
						this._CustomerId = default(System.Nullable<int>);
					}
					this.SendPropertyChanged("Customer");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_SaleInvoice", Storage="_CreatedBy", ThisKey="CreatedById", OtherKey="Id", IsForeignKey=true)]
		public User CreatedBy
		{
			get
			{
				return this._CreatedBy.Entity;
			}
			set
			{
				User previousValue = this._CreatedBy.Entity;
				if (((previousValue != value) 
							|| (this._CreatedBy.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._CreatedBy.Entity = null;
					}
					this._CreatedBy.Entity = value;
					if ((value != null))
					{
						this._CreatedById = value.Id;
					}
					else
					{
						this._CreatedById = default(int);
					}
					this.SendPropertyChanged("CreatedBy");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="SaleInvoice_SaleInvoiceLine", Storage="_Lines", ThisKey="Id", OtherKey="SaleInvoiceId")]
		public EntitySet<SaleInvoiceLine> Lines
		{
			get
			{
				return this._Lines;
			}
			set
			{
				this._Lines.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Lines(SaleInvoiceLine entity)
		{
			this.SendPropertyChanging();
			entity.SaleInvoice = this;
		}
		
		private void detach_Lines(SaleInvoiceLine entity)
		{
			this.SendPropertyChanging();
			entity.SaleInvoice = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.SaleInvoiceLines")]
	public partial class SaleInvoiceLine : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

		private int _Id;
		private int _SaleInvoiceId;
		private int _ProductId;
		private string _ProductName;
		private int _Quantity;
		private decimal _UnitPrice;
		private decimal _LineTotal;
		private EntityRef<SaleInvoice> _SaleInvoice;
		private EntityRef<Product> _Product;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnSaleInvoiceIdChanging(int value);
    partial void OnSaleInvoiceIdChanged();
    partial void OnProductIdChanging(int value);
    partial void OnProductIdChanged();
    partial void OnProductNameChanging(string value);
    partial void OnProductNameChanged();
    partial void OnQuantityChanging(int value);
    partial void OnQuantityChanged();
    partial void OnUnitPriceChanging(decimal value);
    partial void OnUnitPriceChanged();
    partial void OnLineTotalChanging(decimal value);
    partial void OnLineTotalChanged();
    #endregion
		
		public SaleInvoiceLine()
		{
			this._SaleInvoice = default(EntityRef<SaleInvoice>);
			this._Product = default(EntityRef<Product>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SaleInvoiceId", DbType="Int NOT NULL")]
		public int SaleInvoiceId
		{
			get
			{
				return this._SaleInvoiceId;
			}
			set
			{
				if ((this._SaleInvoiceId != value))
				{
					if (this._SaleInvoice.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnSaleInvoiceIdChanging(value);
					this.SendPropertyChanging();
					this._SaleInvoiceId = value;
					this.SendPropertyChanged("SaleInvoiceId");
					this.OnSaleInvoiceIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ProductId", DbType="Int NOT NULL")]
		public int ProductId
		{
			get
			{
				return this._ProductId;
			}
			set
			{
				if ((this._ProductId != value))
				{
					if (this._Product.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnProductIdChanging(value);
					this.SendPropertyChanging();
					this._ProductId = value;
					this.SendPropertyChanged("ProductId");
					this.OnProductIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ProductName", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string ProductName
		{
			get
			{
				return this._ProductName;
			}
			set
			{
				if ((this._ProductName != value))
				{
					this.OnProductNameChanging(value);
					this.SendPropertyChanging();
					this._ProductName = value;
					this.SendPropertyChanged("ProductName");
					this.OnProductNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Quantity", DbType="Int NOT NULL")]
		public int Quantity
		{
			get
			{
				return this._Quantity;
			}
			set
			{
				if ((this._Quantity != value))
				{
					this.OnQuantityChanging(value);
					this.SendPropertyChanging();
					this._Quantity = value;
					this.SendPropertyChanged("Quantity");
					this.OnQuantityChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UnitPrice", DbType="Decimal(18,2) NOT NULL")]
		public decimal UnitPrice
		{
			get
			{
				return this._UnitPrice;
			}
			set
			{
				if ((this._UnitPrice != value))
				{
					this.OnUnitPriceChanging(value);
					this.SendPropertyChanging();
					this._UnitPrice = value;
					this.SendPropertyChanged("UnitPrice");
					this.OnUnitPriceChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LineTotal", DbType="Decimal(18,2) NOT NULL")]
		public decimal LineTotal
		{
			get
			{
				return this._LineTotal;
			}
			set
			{
				if ((this._LineTotal != value))
				{
					this.OnLineTotalChanging(value);
					this.SendPropertyChanging();
					this._LineTotal = value;
					this.SendPropertyChanged("LineTotal");
					this.OnLineTotalChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="SaleInvoice_SaleInvoiceLine", Storage="_SaleInvoice", ThisKey="SaleInvoiceId", OtherKey="Id", IsForeignKey=true)]
		public SaleInvoice SaleInvoice
		{
			get
			{
				return this._SaleInvoice.Entity;
			}
			set
			{
				SaleInvoice previousValue = this._SaleInvoice.Entity;
				if (((previousValue != value) 
							|| (this._SaleInvoice.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._SaleInvoice.Entity = null;
						previousValue.Lines.Remove(this);
					}
					this._SaleInvoice.Entity = value;
					if ((value != null))
					{
						value.Lines.Add(this);
						this._SaleInvoiceId = value.Id;
					}
					else
					{
						this._SaleInvoiceId = default(int);
					}
					this.SendPropertyChanged("SaleInvoice");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Product_SaleInvoiceLine", Storage="_Product", ThisKey="ProductId", OtherKey="Id", IsForeignKey=true)]
		public Product Product
		{
			get
			{
				return this._Product.Entity;
			}
			set
			{
				Product previousValue = this._Product.Entity;
				if (((previousValue != value) 
							|| (this._Product.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Product.Entity = null;
					}
					this._Product.Entity = value;
					if ((value != null))
					{
						this._ProductId = value.Id;
					}
					else
					{
						this._ProductId = default(int);
					}
					this.SendPropertyChanged("Product");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.StockMovements")]
	public partial class StockMovement : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

		private int _Id;
		private System.DateTime _CreatedAt;
		private int _ProductId;
		private string _ProductName;
		private StockMovementKind _Kind;
		private int _Quantity;
		private int _StockBefore;
		private int _StockAfter;
		private string _ReferenceNumber;
		private int _CreatedById;
		private System.Nullable<int> _PurchaseReceiptId;
		private System.Nullable<int> _SaleInvoiceId;
		private string _Note;
		
		private EntityRef<Product> _Product;
		private EntityRef<User> _CreatedBy;
		private EntityRef<PurchaseReceipt> _PurchaseReceipt;
		private EntityRef<SaleInvoice> _SaleInvoice;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnCreatedAtChanging(System.DateTime value);
    partial void OnCreatedAtChanged();
    partial void OnProductIdChanging(int value);
    partial void OnProductIdChanged();
    partial void OnProductNameChanging(string value);
    partial void OnProductNameChanged();
    partial void OnKindChanging(StockMovementKind value);
    partial void OnKindChanged();
    partial void OnQuantityChanging(int value);
    partial void OnQuantityChanged();
    partial void OnStockBeforeChanging(int value);
    partial void OnStockBeforeChanged();
    partial void OnStockAfterChanging(int value);
    partial void OnStockAfterChanged();
    partial void OnReferenceNumberChanging(string value);
    partial void OnReferenceNumberChanged();
    partial void OnCreatedByIdChanging(int value);
    partial void OnCreatedByIdChanged();
    partial void OnPurchaseReceiptIdChanging(System.Nullable<int> value);
    partial void OnPurchaseReceiptIdChanged();
    partial void OnSaleInvoiceIdChanging(System.Nullable<int> value);
    partial void OnSaleInvoiceIdChanged();
    partial void OnNoteChanging(string value);
    partial void OnNoteChanged();
    #endregion
		
		public StockMovement()
		{
			this._Product = default(EntityRef<Product>);
			this._CreatedBy = default(EntityRef<User>);
			this._PurchaseReceipt = default(EntityRef<PurchaseReceipt>);
			this._SaleInvoice = default(EntityRef<SaleInvoice>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CreatedAt", DbType="DateTime2 NOT NULL")]
		public System.DateTime CreatedAt
		{
			get
			{
				return this._CreatedAt;
			}
			set
			{
				if ((this._CreatedAt != value))
				{
					this.OnCreatedAtChanging(value);
					this.SendPropertyChanging();
					this._CreatedAt = value;
					this.SendPropertyChanged("CreatedAt");
					this.OnCreatedAtChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ProductId", DbType="Int NOT NULL")]
		public int ProductId
		{
			get
			{
				return this._ProductId;
			}
			set
			{
				if ((this._ProductId != value))
				{
					if (this._Product.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnProductIdChanging(value);
					this.SendPropertyChanging();
					this._ProductId = value;
					this.SendPropertyChanged("ProductId");
					this.OnProductIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ProductName", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string ProductName
		{
			get
			{
				return this._ProductName;
			}
			set
			{
				if ((this._ProductName != value))
				{
					this.OnProductNameChanging(value);
					this.SendPropertyChanging();
					this._ProductName = value;
					this.SendPropertyChanged("ProductName");
					this.OnProductNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Kind", DbType="Int NOT NULL")]
		public StockMovementKind Kind
		{
			get
			{
				return this._Kind;
			}
			set
			{
				if ((this._Kind != value))
				{
					this.OnKindChanging(value);
					this.SendPropertyChanging();
					this._Kind = value;
					this.SendPropertyChanged("Kind");
					this.OnKindChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Quantity", DbType="Int NOT NULL")]
		public int Quantity
		{
			get
			{
				return this._Quantity;
			}
			set
			{
				if ((this._Quantity != value))
				{
					this.OnQuantityChanging(value);
					this.SendPropertyChanging();
					this._Quantity = value;
					this.SendPropertyChanged("Quantity");
					this.OnQuantityChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_StockBefore", DbType="Int NOT NULL")]
		public int StockBefore
		{
			get
			{
				return this._StockBefore;
			}
			set
			{
				if ((this._StockBefore != value))
				{
					this.OnStockBeforeChanging(value);
					this.SendPropertyChanging();
					this._StockBefore = value;
					this.SendPropertyChanged("StockBefore");
					this.OnStockBeforeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_StockAfter", DbType="Int NOT NULL")]
		public int StockAfter
		{
			get
			{
				return this._StockAfter;
			}
			set
			{
				if ((this._StockAfter != value))
				{
					this.OnStockAfterChanging(value);
					this.SendPropertyChanging();
					this._StockAfter = value;
					this.SendPropertyChanged("StockAfter");
					this.OnStockAfterChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ReferenceNumber", DbType="NVarChar(40) NOT NULL", CanBeNull=false)]
		public string ReferenceNumber
		{
			get
			{
				return this._ReferenceNumber;
			}
			set
			{
				if ((this._ReferenceNumber != value))
				{
					this.OnReferenceNumberChanging(value);
					this.SendPropertyChanging();
					this._ReferenceNumber = value;
					this.SendPropertyChanged("ReferenceNumber");
					this.OnReferenceNumberChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CreatedById", DbType="Int NOT NULL")]
		public int CreatedById
		{
			get
			{
				return this._CreatedById;
			}
			set
			{
				if ((this._CreatedById != value))
				{
					if (this._CreatedBy.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnCreatedByIdChanging(value);
					this.SendPropertyChanging();
					this._CreatedById = value;
					this.SendPropertyChanged("CreatedById");
					this.OnCreatedByIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PurchaseReceiptId", DbType="Int")]
		public System.Nullable<int> PurchaseReceiptId
		{
			get
			{
				return this._PurchaseReceiptId;
			}
			set
			{
				if ((this._PurchaseReceiptId != value))
				{
					if (this._PurchaseReceipt.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnPurchaseReceiptIdChanging(value);
					this.SendPropertyChanging();
					this._PurchaseReceiptId = value;
					this.SendPropertyChanged("PurchaseReceiptId");
					this.OnPurchaseReceiptIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SaleInvoiceId", DbType="Int")]
		public System.Nullable<int> SaleInvoiceId
		{
			get
			{
				return this._SaleInvoiceId;
			}
			set
			{
				if ((this._SaleInvoiceId != value))
				{
					if (this._SaleInvoice.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnSaleInvoiceIdChanging(value);
					this.SendPropertyChanging();
					this._SaleInvoiceId = value;
					this.SendPropertyChanged("SaleInvoiceId");
					this.OnSaleInvoiceIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Note", DbType="NVarChar(200)")]
		public string Note
		{
			get
			{
				return this._Note;
			}
			set
			{
				if ((this._Note != value))
				{
					this.OnNoteChanging(value);
					this.SendPropertyChanging();
					this._Note = value;
					this.SendPropertyChanged("Note");
					this.OnNoteChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Product_StockMovement", Storage="_Product", ThisKey="ProductId", OtherKey="Id", IsForeignKey=true)]
		public Product Product
		{
			get
			{
				return this._Product.Entity;
			}
			set
			{
				Product previousValue = this._Product.Entity;
				if (((previousValue != value) 
							|| (this._Product.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Product.Entity = null;
					}
					this._Product.Entity = value;
					if ((value != null))
					{
						this._ProductId = value.Id;
					}
					else
					{
						this._ProductId = default(int);
					}
					this.SendPropertyChanged("Product");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="User_StockMovement", Storage="_CreatedBy", ThisKey="CreatedById", OtherKey="Id", IsForeignKey=true)]
		public User CreatedBy
		{
			get
			{
				return this._CreatedBy.Entity;
			}
			set
			{
				User previousValue = this._CreatedBy.Entity;
				if (((previousValue != value) 
							|| (this._CreatedBy.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._CreatedBy.Entity = null;
					}
					this._CreatedBy.Entity = value;
					if ((value != null))
					{
						this._CreatedById = value.Id;
					}
					else
					{
						this._CreatedById = default(int);
					}
					this.SendPropertyChanged("CreatedBy");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="PurchaseReceipt_StockMovement", Storage="_PurchaseReceipt", ThisKey="PurchaseReceiptId", OtherKey="Id", IsForeignKey=true)]
		public PurchaseReceipt PurchaseReceipt
		{
			get
			{
				return this._PurchaseReceipt.Entity;
			}
			set
			{
				PurchaseReceipt previousValue = this._PurchaseReceipt.Entity;
				if (((previousValue != value) 
							|| (this._PurchaseReceipt.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._PurchaseReceipt.Entity = null;
					}
					this._PurchaseReceipt.Entity = value;
					if ((value != null))
					{
						this._PurchaseReceiptId = value.Id;
					}
					else
					{
						this._PurchaseReceiptId = default(System.Nullable<int>);
					}
					this.SendPropertyChanged("PurchaseReceipt");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="SaleInvoice_StockMovement", Storage="_SaleInvoice", ThisKey="SaleInvoiceId", OtherKey="Id", IsForeignKey=true)]
		public SaleInvoice SaleInvoice
		{
			get
			{
				return this._SaleInvoice.Entity;
			}
			set
			{
				SaleInvoice previousValue = this._SaleInvoice.Entity;
				if (((previousValue != value) 
							|| (this._SaleInvoice.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._SaleInvoice.Entity = null;
					}
					this._SaleInvoice.Entity = value;
					if ((value != null))
					{
						this._SaleInvoiceId = value.Id;
					}
					else
					{
						this._SaleInvoiceId = default(System.Nullable<int>);
					}
					this.SendPropertyChanged("SaleInvoice");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
}
#pragma warning restore 1591
