using System;
using System . Collections . Generic;
using System . Text;

namespace NewWpfDev
{
    public  class ValidTableNames
    {
        public static List<string> ADVENTURENAMES = new List<string> ( );
        public static List<string> fullnames = new List<string> ( );

        #region AdventureWorks Table name support
        public ValidTableNames ( )
        {
            ADVENTURENAMES = LoadAWNames ( );
        }
        public static List<string >ConvertTableName ( List<string> tablenames )
        {
            if( ADVENTURENAMES .Count == 0 )
                ADVENTURENAMES = LoadAWNames ( );
            fullnames . Clear ( );
            foreach ( var item in tablenames )
            {
                for ( int x = 0 ; x < ADVENTURENAMES . Count ; x++ )
                {
                    if ( ADVENTURENAMES [ x ] . Contains ( item ) )
                    {
                        fullnames . Add ( ADVENTURENAMES [ x ] );
                        break;
                    }
                }
            }
            return fullnames;
        }
        private static List<string> LoadAWNames()
        {
            List<string> names = new List<string>();
            names . Add ( "HumanResources.Department" );
            names . Add ( "HumanResources.Employee" );
            names . Add ( "HumanResources.EmployeeDepartmentHistory" );
            names . Add ( "HumanResources.EmployeePayHistory" );
            names . Add ( "HumanResources.JobCandidate" );
            names . Add ( "HumanResources.Shift" );
            names . Add ( "Person.Address" );
            names . Add ( "Person.AddressType" );
            names . Add ( "Person.BusinessEntity" );
            names . Add ( "Person.BusinessEntityAddress" );
            names . Add ( "Person.BusinessEntityContact" );
            names . Add ( "Person.ContactType" );
            names . Add ( "Person.CountryRegion" );
            names . Add ( "Person.EmailAddress" );
            names . Add ( "Person.Password" );
            names . Add ( "Person.Person" );
            names . Add ( "Person.P:ersonPhone" );
            names . Add ( "Person.PhoneNumberType" );
            names . Add ( "Person.StateProvince" );
            names . Add ( "Production.BillOfMaterials" );
            names . Add ( "Production.Culture" );
            names . Add ( "Production.Document" );
            names . Add ( "Production.Illustration" );
            names . Add ( "Production.Location" );
            names . Add ( "Production.Product" );
            names . Add ( "Production.ProductCategory" );
            names . Add ( "Production.ProductCostHistory" );
            names . Add ( "Production.ProductDescription" );
            names . Add ( "Production.ProductDocument" );
            names . Add ( "Production.ProductInventory" );
            names . Add ( "Production.ProductListPriceHistory" );
            names . Add ( "Production.ProductModel" );
            names . Add ( "Production.ProductModelIlustration" );
            names . Add ( "Production.ProductModelProductDescriptionCulture" );
            names . Add ( "Production.ProductPhoto" );
            names . Add ( "Production.ProductProductPhoto" );
            names . Add ( "Production.ProductReview" );
            names . Add ( "Production.ProductSubCategory" );
            names . Add ( "Production.ScrapReason" );
            names . Add ( "Production.TransactionHistory" );
            names . Add ( "Production.TransactionHistoryArchive" );
            names . Add ( "Production.UnitMeasure" );
            names . Add ( "Production.WorkOrder" );
            names . Add ( "Production.WorkOrderRouting" );
            names . Add ( "Purchasing.ProducrtVendor" );
            names . Add ( "Purchasing.PurchaseOrderDetail" );
            names . Add ( "Purchasing.PurchaseOrderHeader" );
            names . Add ( "Purchasing.ShipMethod" );
            names . Add ( "Purchasing.Vendor" );
            names . Add ( "Sales.CountryRegionCurrency" );
            names . Add ( "Sales.CreditCard" );
            names . Add ( "Sales.Currency" );
            names . Add ( "Sales.Customer" );
            names . Add ( "Sales.PersonCreditCard" );
            names . Add ( "Sales.SalesOrderDetail" );
            names . Add ( "Sales.SalesOrderHeader" );
            names . Add ( "Sales.SalesOrderHeaderSalesReason" );
            names . Add ( "Sales.SalesPerson" );
            names . Add ( "Sales.SalesPersonQuotaHeader" );
            names . Add ( "Sales.SalesReason" );
            names . Add ( "Sales.SalesTaxRate" );
            names . Add ( "Sales.SalesTerritory" );
            names . Add ( "Sales.SalesTerritoryHistory" );
            names . Add ( "Sales.ShoppingCartHistory" );
            names . Add ( "Sales.SpecialOffer" );
            names . Add ( "Sales.SpecialOfferProduct" );
            names . Add ( "Sales.Store" );
            return names;
        }
        #endregion AdvenntureWorks Table name support

    }
}
