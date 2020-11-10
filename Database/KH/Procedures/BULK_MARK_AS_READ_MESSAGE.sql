/* Definition for the BULK_MARK_AS_READ_MESSAGE procedure :  */

------------------------------------------------------------------------------
-- Create date: 2020-02-15
-- Autor: Luis Lema
--
-- Description: 
-- Mark messages as read as a whole
--
-- Parameters:
-- LAST_MESSAGE_ID - The last message to be marked, all earlier ones 
--                   will be marked too
------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE BULK_MARK_AS_READ_MESSAGE
(
  SENDER_PEER TYPE OF COLUMN PEER.TOKEN,
  LAST_MESSAGE_TIMEID TYPE OF COLUMN MESSAGE.TIMEID
)
AS
DECLARE VARIABLE ID_SENDER INTEGER;
BEGIN
    ID_SENDER=(SELECT ID FROM PEER WHERE TOKEN=:SENDER_PEER);
    
    UPDATE MESSAGE
    SET READ_STATE='TRUE'
    WHERE
    TIMEID<=:LAST_MESSAGE_TIMEID
    AND READ_STATE='FALSE'
    AND ID_SENDER=:ID_SENDER;
END