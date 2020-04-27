/* Definition for the SAVE_NOTIFICATION procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Save a notification for later use.
--
-- Parameters:
-- TOKEN - The token of peer that fired the notification.
-- TYPE_NOT - The type of notification.
-- CONTENT - Summary of notification.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SAVE_NOTIFICATION
(
  ID TYPE OF COLUMN NOTIFICATION.ID,
  TYPE_NOTF TYPE OF COLUMN NOTIFICATION.TYPE_NOTF,
  CONTENT TYPE OF COLUMN NOTIFICATION.CONTENT
)
AS

DECLARE VARIABLE TODAY TIMESTAMP;

BEGIN
   TODAY=(SELECT CURRENT_TIMESTAMP FROM RDB$DATABASE);
   
   INSERT INTO NOTIFICATION(ID,REG_DATE,TYPE_NOTF,CONTENT)
   VALUES(:ID,:TODAY,:TYPE_NOTF,:CONTENT);
END