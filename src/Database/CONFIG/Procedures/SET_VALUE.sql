/* Definition for the SET_VALUE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Set the value of a configuration.
--
-- Parameters:
-- IDKEY - The name of configuration, this value is unique across others.
-- VAL - The value.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SET_VALUE(
  IDKEY TYPE OF COLUMN APPCONFIG.IDKEY,
  VAL TYPE OF COLUMN APPCONFIG.VAL)
AS
BEGIN 
   UPDATE OR INSERT INTO APPCONFIG(IDKEY,VAL)
   VALUES(:IDKEY,:VAL)
   MATCHING(IDKEY);
END