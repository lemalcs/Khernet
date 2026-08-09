/* Definition for the GET_STR procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the value of a connection string.
--
-- Parameters:
-- IDKEY - The name of connection string, this value is unique across others.
--
-- Returns:
-- The value of connection string.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_STR
(
  IDKEY TYPE OF COLUMN CONNSTR.IDKEY  
)
RETURNS
(
  VAL TYPE OF COLUMN CONNSTR.STR
)
AS
BEGIN
   FOR SELECT STR FROM CONNSTR
   WHERE
   IDKEY=:IDKEY
   INTO :VAL
   DO SUSPEND;
END