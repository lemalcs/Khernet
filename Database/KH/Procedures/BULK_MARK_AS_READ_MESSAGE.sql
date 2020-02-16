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
  LAST_MESSAGE_ID TYPE OF COLUMN MESSAGE.ID
)
AS
DECLARE VARIABLE ID_SENDER INTEGER;
BEGIN
    ID_SENDER=(SELECT ID FROM MESSAGE WHERE ID=:LAST_MESSAGE_ID);
    
    UPDATE MESSAGE
    SET READ_STATE='TRUE'
    WHERE
    ID<=:LAST_MESSAGE_ID
    AND READ_STATE='FALSE';
    
    DELETE FROM NOTIFICATION
    WHERE
    ID_TOKEN=:ID_SENDER
    AND PROCESSED='FALSE'
    AND TYPE_NOT=0;
    
END