USE [IAN1]
GO

/****** Object: SqlProcedure [dbo].[spGetColsCount] Script Date: 08/11/2022 09:27:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- THIS WORKING JUST FINE 6/11/2022
-- ***** do not touch *****
Alter PROCEDURE [dbo].[spGetColsCount]
  @arg1 varchar(255)
	,@Result varchar(25) output

AS
BEGIN
  SET NOCOUNT Off;
		-- ***** do not touch *****
  DECLARE @sql nvarchar(max)		
		DROP TABLE if exists cols

			SELECT Table_name, column_name, data_type into cols FROM INFORMATION_SCHEMA.COLUMNS 
			where table_name = @arg1
			order by TABLE_NAME ASC 
	
	Select @result = count(*)  from cols
	print @result

	
END
