using DapperGenericsLib;

using NewWpfDev . UserControls;
using NewWpfDev. ViewModels;

using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Media;
//using DapperGenericsLib;
namespace NewWpfDev
{
	public static class DataExtensions
	{
        /// <summary>
        /// Special method that loads GENERIC format data from Sql Table, & Loads it into specified DataGrid
        /// cleaning away unused columns and even loading the real Column names  for the selected table
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="Tablename"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static ObservableCollection <DapperGenericsLib.GenericClass >LoadGenData( this ObservableCollection<DapperGenericsLib . GenericClass>  collection, string Tablename, DataGrid  grid)
        {
            GenericGridControl genctrl = new GenericGridControl (null );
            Task .Run(() => DataLoad. LoadGenericTable ( Tablename ,  grid, collection ));
            return collection;
        }

	}

}
