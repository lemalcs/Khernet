/* Definition for the GET_VALUE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the value of a configuration.
--
-- Parameters:
-- IDKEY - The name of configuration, this value is unique across others.
--
-- Returns:
-- The value of configuration.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_VALUE
(
  IDKEY TYPE OF COLUMN APPCONFIG.IDKEY
)
RETURNS
(
  RESULT TYPE OF COLUMN APPCONFIG.VAL
)
AS
BEGIN
   FOR SELECT VAL FROM APPCONFIG
   WHERE
   IDKEY=:IDKEY
   INTO
   :RESULT
   DO SUSPEND; 
END