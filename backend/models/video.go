package models

import (
	"github.com/bwmarrin/snowflake"
)

type VideoStatus int8

const (
	CREATED VideoStatus = iota
	UPLOADING
	PROCESSING
	COMPLETE
)

type Visibility int8

const (
	PRIVATE Visibility = iota
	UNLISTED
	PUBLIC
)

type Metadata struct {
	Status       VideoStatus
	MimeType     string `gorm:"type:text"`
	DurationSecs int32
}

type Settings struct {
	CreatorId   int64
	Creator     User
	Name        string `gorm:"type:text"`
	Description string `gorm:"type:text"`
	Visibility  Visibility
}

type Statistics struct {
	Views    int64
	Likes    int64
	Dislikes int64
}

type Video struct {
	Base
	Metadata
	Settings
	Statistics
}

func NewVideo(creator *User) (*Video, error) {
	video := Video{
		Base: Base{
			Id: idgen.Generate().Int64(),
		},
		Metadata: Metadata{
			Status: CREATED,
		},
		Settings: Settings{
			CreatorId:  creator.Id,
			Name:       "Untitled Video",
			Visibility: PUBLIC,
		},
	}

	err := db.Create(&video).Error

	return &video, err
}

func FetchVideo(id snowflake.ID) (*Video, error) {
	video := Video{}
	err := db.Preload("Creator").First(&video, id.Int64()).Error
	return &video, err
}

func FetchAllVideos() ([]Video, error) {
	var videos []Video
	err := db.Preload("Creator").
		Order("id DESC").
		Find(&videos, &Video{Metadata: Metadata{Status: COMPLETE}, Settings: Settings{Visibility: PUBLIC}}).
		Error
	return videos, err
}

func (video *Video) IdSnowflake() snowflake.ID {
	return snowflake.ParseInt64(video.Id)
}

func (video *Video) IsCreator(user *User) bool {
	return video.CreatorId == user.Id
}

func (video *Video) Save() error {
	return db.Save(video).Error
}
