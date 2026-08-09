/* Definition for the GET_NOTIFICATION_DETAIL procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the details of specified notification.
--
-- Parameters:
-- ID - The id of notification.
--
-- Returns:
-- The metadata  of notification.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_NOTIFICATION_DETAIL
(
  ID TYPE OF COLUMN NOTIFICATION.ID
)
RETURNS
(
  CONTENT TYPE OF COLUMN NOTIFICATION.CONTENT
)
AS
BEGIN
   FOR SELECT CONTENT FROM NOTIFICATION
   WHERE
   ID=:ID
   INTO :CONTENT
   DO SUSPEND;
END