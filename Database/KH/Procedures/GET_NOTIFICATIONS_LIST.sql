/* Definition for the GET_NOTIFICATIONS_LIST procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the list OF notifications.
--
-- Returns:
-- The list of id and type of notifications.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_NOTIFICATIONS_LIST
RETURNS
(
  ID TYPE OF COLUMN NOTIFICATION.ID,
  TYPE_NOTF TYPE OF COLUMN NOTIFICATION.TYPE_NOTF
)
AS
BEGIN
   FOR SELECT ID,TYPE_NOTF FROM NOTIFICATION
   INTO :ID,:TYPE_NOTF
   DO SUSPEND;
END