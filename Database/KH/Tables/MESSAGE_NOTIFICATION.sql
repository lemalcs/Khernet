/* Structure for the MESSAGE_NOTIFICATION table :  */

CREATE TABLE MESSAGE_NOTIFICATION 
(
  ID_SENDER INTEGER NOT NULL,
  ID_MESSAGE INTEGER NOT NULL,
  CONTENT_TYPE SMALLINT NOT NULL,
  REG_DATE TIMESTAMP default CURRENT_TIMESTAMP NOT NULL
);


ALTER TABLE MESSAGE_NOTIFICATION ADD CONSTRAINT PK_MESSAGE_NOTIFICATION PRIMARY KEY (ID_SENDER,ID_MESSAGE);