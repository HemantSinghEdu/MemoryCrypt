CREATE TABLE Articles
(
	Id UNIQUEIDENTIFIER primary key Default(NewId()),
    Title varchar(max),
    Author varchar(max),
    Content varchar(max),
    Views int,
    UpVotes int
)