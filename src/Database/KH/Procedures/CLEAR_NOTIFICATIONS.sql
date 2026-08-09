/* Definition for the CLEAR_NOTIFICATIONS procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Deletes all notifications.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE CLEAR_NOTIFICATIONS
AS
BEGIN 
   DELETE FROM NOTIFICATION;
END