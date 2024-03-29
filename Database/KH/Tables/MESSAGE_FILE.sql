/* Structure for the MESSAGE_FILE table :  */

CREATE TABLE MESSAGE_FILE 
(
  ID BIGINT NOT NULL,
  ID_MESSAGE BIGINT,
  CONTENT BLOB,
  THUMBN BLOB,
  FPATH BLOB
);

ALTER TABLE MESSAGE_FILE ADD PRIMARY KEY (ID);

ALTER TABLE MESSAGE_FILE ADD CONSTRAINT FK_MESSAGE_FILE_MESSAGE FOREIGN KEY (ID_MESSAGE) REFERENCES MESSAGE(ID);