import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import { Calendar, Circle, MessageSquare, Tag } from 'lucide-react';
import { format } from 'date-fns';

export enum IssuePriority {
  HIGH = 0,
  MEDIUM = 1,
  LOW = 2
}

export const PRIORITY_CONFIG = {
  [IssuePriority.HIGH]: {
    label: 'High',
    color: 'bg-red-100 text-red-700 border-red-200'
  },
  [IssuePriority.MEDIUM]: {
    label: 'Medium',
    color: 'bg-yellow-100 text-yellow-700 border-yellow-200'
  },
  [IssuePriority.LOW]: {
    label: 'Low',
    color: 'bg-blue-100 text-blue-700 border-blue-200'
  }
};

interface Label {
  id: number;
  projectId: number;
  name: string;
  color: string;
  createdAt: string;
  updatedAt: string;
}

interface Issue {
  id: number;
  projectId: number;
  projectName: string;
  statusId: number;
  statusName: string;
  statusColor: string;
  title: string;
  description?: string | null;
  ownerId: number;
  ownerName: string;
  ownerEmail: string;
  assigneeId?: number | null;
  assigneeName?: string | null;
  assigneeEmail?: string | null;
  dueDate?: string | null;
  priority: IssuePriority;
  position: number;
  labels: Label[];
  subtaskCount: number;
  completedSubtaskCount: number;
  commentCount: number;
  createdAt: string;
  updatedAt: string;
}

interface IssueCardProps {
  issue: Issue;
  onClick?: (issue: Issue) => void;
  onDragStart?: (e: React.DragEvent, issue: Issue) => void;
  onDragEnd?: (e: React.DragEvent) => void;
  isDragging?: boolean;
}

export default function IssueCard({
  issue,
  onClick,
  onDragStart,
  onDragEnd,
  isDragging = false
}: IssueCardProps) {
  const priorityConfig =
    PRIORITY_CONFIG[issue.priority] || PRIORITY_CONFIG[IssuePriority.MEDIUM];
  const isOverdue = issue.dueDate && new Date(issue.dueDate) < new Date();
  const subtaskProgress =
    issue.subtaskCount > 0
      ? Math.round((issue.completedSubtaskCount / issue.subtaskCount) * 100)
      : 0;

  const getInitials = (name: string) => {
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  };

  return (
    <Card
      className={`mb-3 cursor-pointer transition-all hover:shadow-md ${
        isDragging ? 'opacity-50' : ''
      }`}
      draggable
      onDragStart={(e) => {
        onDragStart?.(e, issue);
        e.dataTransfer.effectAllowed = 'move';
      }}
      onDragEnd={onDragEnd}
      onClick={() => onClick?.(issue)}
    >
      <CardContent className="space-y-3 p-4">
        {/* Title and Priority */}
        <div className="flex items-start justify-between gap-2">
          <h4 className="line-clamp-2 flex-1 text-sm font-semibold">
            {issue.title}
          </h4>
          <Badge
            variant="outline"
            className={`text-xs ${priorityConfig.color}`}
          >
            {priorityConfig.label}
          </Badge>
        </div>

        {/* Description */}
        {issue.description && (
          <p className="line-clamp-2 text-xs text-muted-foreground">
            {issue.description}
          </p>
        )}

        {/* Labels */}
        {issue.labels && issue.labels.length > 0 && (
          <div className="flex flex-wrap gap-1">
            {issue.labels.slice(0, 3).map((label) => (
              <Badge
                key={label.id}
                variant="outline"
                className="text-xs"
                style={{
                  backgroundColor: `${label.color}20`,
                  borderColor: label.color,
                  color: label.color
                }}
              >
                <Tag className="mr-1 h-2 w-2" />
                {label.name}
              </Badge>
            ))}
            {issue.labels.length > 3 && (
              <Badge variant="outline" className="text-xs">
                +{issue.labels.length - 3}
              </Badge>
            )}
          </div>
        )}

        {/* Subtask Progress */}
        {issue.subtaskCount > 0 && (
          <div className="space-y-1">
            <div className="flex items-center justify-between text-xs">
              <span className="text-muted-foreground">Subtasks</span>
              <span className="text-muted-foreground">
                {issue.completedSubtaskCount}/{issue.subtaskCount}
              </span>
            </div>
            <div className="h-1.5 w-full overflow-hidden rounded-full bg-gray-200">
              <div
                className="h-full bg-primary transition-all"
                style={{ width: `${subtaskProgress}%` }}
              />
            </div>
          </div>
        )}

        {/* Footer */}
        <div className="flex items-center justify-between border-t pt-2">
          <div className="flex items-center gap-2">
            {issue.assigneeId ? (
              <Avatar className="h-6 w-6">
                <AvatarFallback className="text-xs">
                  {getInitials(
                    issue.assigneeName || issue.assigneeEmail || 'U'
                  )}
                </AvatarFallback>
              </Avatar>
            ) : (
              <div className="flex h-6 w-6 items-center justify-center rounded-full border-2 border-dashed border-muted-foreground">
                <Circle className="h-3 w-3 text-muted-foreground" />
              </div>
            )}
            {issue.commentCount > 0 && (
              <div className="flex items-center gap-1 text-xs text-muted-foreground">
                <MessageSquare className="h-3 w-3" />
                <span>{issue.commentCount}</span>
              </div>
            )}
          </div>

          {issue.dueDate && (
            <div
              className={`flex items-center gap-1 text-xs ${
                isOverdue ? 'font-medium text-red-600' : 'text-muted-foreground'
              }`}
            >
              <Calendar className="h-3 w-3" />
              <span>{format(new Date(issue.dueDate), 'MMM d')}</span>
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  );
}
