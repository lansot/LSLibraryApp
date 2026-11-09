export interface TopicDto {
  id: number;
  title: string;
  body: string;
  tags: TopicTag[];
  updatedAt: Date;
  version: number;
}

export interface TopicTag {
  name: string;
}
