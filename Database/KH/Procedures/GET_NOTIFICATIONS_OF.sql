/* Definition for the GET_NOTIFICATIONS_OF procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Get the list of notification fired be a peer.
--
-- Parameters:
-- SENDER_TOKEN - The token of peer.
--
-- Returns:
-- The list of notificaions.
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE GET_NOTIFICATIONS_OF
(
  SENDER_TOKEN TYPE OF COLUMN PEER.TOKEN
)
RETURNS
(
  CONTENT TYPE OF COLUMN NOTIFICATION.CONTENT,
  REG_DATE TYPE OF COLUMN NOTIFICATION.REG_DATE
)
AS
BEGIN
   FOR SELECT A.CONTENT,A.REG_DATE 
   FROM NOTIFICATION A JOIN PEER B
   ON A.ID_TOKEN=B.ID
   WHERE
   B.TOKEN=:SENDER_TOKEN
   INTO :CONTENT,:REG_DATE
   DO SUSPEND;
END