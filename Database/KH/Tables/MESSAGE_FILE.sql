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