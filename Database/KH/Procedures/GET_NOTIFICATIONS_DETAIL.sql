/* Definition for the GET_NOTIFICATIONS_DETAIL procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the details of notifications.
--
-- Returns:
-- The list of metadata of notifications.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_NOTIFICATIONS_DETAIL
RETURNS
(
  TOKEN TYPE OF COLUMN PEER.TOKEN,
  TYPENOT TYPE OF COLUMN NOTIFICATION.TYPE_NOT,
  CONTENT TYPE OF COLUMN NOTIFICATION.CONTENT
)
AS
BEGIN
   FOR SELECT B.TOKEN,A.TYPE_NOT,A.CONTENT FROM NOTIFICATION A JOIN PEER B
   ON A.ID_TOKEN=B.ID
   WHERE
   A.PROCESSED='FALSE'
   INTO :TOKEN,:TYPENOT,:CONTENT
   DO SUSPEND;
END