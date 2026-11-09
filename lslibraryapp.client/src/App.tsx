import { useEffect, useState } from "react";
import "./App.css";
import "@mantine/core/styles.css";

import {
  MantineProvider,
  Loader,
  Title,
  Text,
  Center,
  OverflowList,
  Badge,
  HoverCard,
  Table,
  ActionIcon,
} from "@mantine/core";
import { PencilIcon } from "@phosphor-icons/react";
import type { TopicDto } from "./models/topicDto";

function App() {
  const [topics, setTopics] = useState<TopicDto[]>();

  useEffect(() => {
    populateTopicData();
  }, []);

  const contents =
    topics === undefined ? (
      <Loader color="green" />
    ) : (
      <Table>
        <Table.Thead>
          <Table.Tr>
            <Table.Th>Title</Table.Th>
            <Table.Th>Tags</Table.Th>
            <Table.Th>Last Updated</Table.Th>
            <Table.Th>Actions</Table.Th>
          </Table.Tr>
        </Table.Thead>
        <Table.Tbody>
          {topics.map((topic) => (
            <Table.Tr key={topic.id}>
              <Table.Td>{topic.title}</Table.Td>
              <Table.Td>
                {
                  <OverflowList
                    data={topic.tags}
                    gap={4}
                    renderItem={(item, index) => (
                      <Badge key={index}>{item.name}</Badge>
                    )}
                    renderOverflow={(items) => (
                      <Center>
                        <HoverCard>
                          <HoverCard.Target>
                            <Badge>+{items.length} more</Badge>
                          </HoverCard.Target>
                          <HoverCard.Dropdown>
                            {items.map((item, index) => (
                              <div key={index}>{item.name}</div>
                            ))}
                          </HoverCard.Dropdown>
                        </HoverCard>
                      </Center>
                    )}
                  />
                }
              </Table.Td>
              <Table.Td>
                {topic.updatedAt ? topic.updatedAt.toString() : "-"}
              </Table.Td>
              <Table.Td>
                <ActionIcon variant="filled" aria-label="Edit">
                  <PencilIcon>
                    {/* TODO hook this up to an upsert modal component!! */}
                  </PencilIcon>
                </ActionIcon>
              </Table.Td>
            </Table.Tr>
          ))}
        </Table.Tbody>
      </Table>
    );

  return (
    <MantineProvider>
      {
        <div>
          <Title mb="xs" order={1}>
            LIBRARINATOR 9000!!!
          </Title>
          <Text mb="md">
            You've got your Topics and your Books. It's everything a growing boy
            needs.
          </Text>
          <Center>{contents}</Center>
        </div>
      }
    </MantineProvider>
  );

  async function populateTopicData() {
    const response = await fetch("topics");
    if (response.ok) {
      const data = await response.json();
      setTopics(data);
    }
  }
}

export default App;
