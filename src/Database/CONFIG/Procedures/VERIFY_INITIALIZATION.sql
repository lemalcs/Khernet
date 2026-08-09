/* Definition for the VERIFY_INITIALIZATION procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Verify if configurations have been added.
--
-- Returns:
-- True if there are configurations otherwise false.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE VERIFY_INITIALIZATION
RETURNS
(
  IS_INITIALIZED BOOLEAN
)
AS
DECLARE VARIABLE COUNTER INT;
BEGIN
   SELECT COUNT(1)FROM APPCONFIG
   INTO :COUNTER;
   
   COUNTER=COUNTER+(SELECT COUNT(1)FROM CONNSTR);
   
   IF(:COUNTER=0)THEN
   BEGIN
      :IS_INITIALIZED='FALSE';
   END
   ELSE 
   BEGIN
      :IS_INITIALIZED='TRUE';
   END   
   
END