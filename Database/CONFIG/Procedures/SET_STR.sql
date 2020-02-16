/* Definition for the SET_STR procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Set the value of a connection string.
--
-- Parameters:
-- IDKEY - The name of connection string, this value is unique across others.
-- VAL - The connections string.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SET_STR
(
  IDKEY TYPE OF COLUMN CONNSTR.IDKEY,
  VAL TYPE OF COLUMN CONNSTR.STR
)
AS
BEGIN 
   UPDATE OR INSERT INTO CONNSTR(IDKEY,STR)
   VALUES(:IDKEY,:VAL)
   MATCHING(IDKEY);
END