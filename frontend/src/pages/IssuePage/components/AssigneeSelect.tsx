import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import {
  Popover,
  PopoverContent,
  PopoverTrigger
} from '@/components/ui/popover';
import { Button } from '@/components/ui/button';
import { User, UserX } from 'lucide-react';

interface AssigneeSelectProps {
  subtask: {
    id: number;
    assigneeId?: number | null;
    assigneeName?: string | null;
    assigneeEmail?: string | null;
  };
  members: Array<{
    userId: number;
    name: string;
    email: string;
    avatar?: string | null;
  }>;
  onAssign: (assigneeId: number | null) => void;
}

export default function AssigneeSelect({ subtask, members, onAssign }: AssigneeSelectProps) {
  const getInitials = (name: string) => {
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  };

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button variant="ghost" size="sm" className="h-8 w-8 p-0">
          {subtask.assigneeId ? (
            <Avatar className="h-6 w-6">
              <AvatarFallback className="text-xs">
                {getInitials(subtask.assigneeName || subtask.assigneeEmail || 'U')}
              </AvatarFallback>
            </Avatar>
          ) : (
            <div className="h-6 w-6 rounded-full border-2 border-dashed border-muted-foreground flex items-center justify-center">
              <User className="h-3 w-3 text-muted-foreground" />
            </div>
          )}
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-64 p-2">
        <div className="space-y-1">
          <Button
            variant="ghost"
            className="w-full justify-start"
            onClick={() => onAssign(null)}
          >
            <UserX className="h-4 w-4 mr-2" />
            Unassign
          </Button>
          {members.map((member) => (
            <Button
              key={member.userId}
              variant="ghost"
              className="w-full justify-start"
              onClick={() => onAssign(member.userId)}
            >
              <Avatar className="h-6 w-6 mr-2">
                <AvatarFallback className="text-xs">
                  {getInitials(member.name)}
                </AvatarFallback>
              </Avatar>
              <span className="text-sm">{member.name}</span>
            </Button>
          ))}
        </div>
      </PopoverContent>
    </Popover>
  );
}

