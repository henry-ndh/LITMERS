import { Badge } from '@/components/ui/badge';
import {
  Popover,
  PopoverContent,
  PopoverTrigger
} from '@/components/ui/popover';
import { Button } from '@/components/ui/button';

interface StatusSelectProps {
  subtask: {
    id: number;
    isDone: boolean;
  };
  onStatusChange: (isDone: boolean) => void;
}

export default function StatusSelect({ subtask, onStatusChange }: StatusSelectProps) {
  return (
    <Popover>
      <PopoverTrigger asChild>
        <button className="hover:opacity-80 transition-opacity">
          <Badge
            variant="outline"
            className={
              subtask.isDone
                ? 'bg-green-100 text-green-700 border-green-200 cursor-pointer'
                : 'bg-red-100 text-red-700 border-red-200 cursor-pointer'
            }
          >
            {subtask.isDone ? 'DONE' : 'TO DO'}
          </Badge>
        </button>
      </PopoverTrigger>
      <PopoverContent className="w-48 p-2">
        <div className="space-y-1">
          <Button
            variant="ghost"
            className="w-full justify-start"
            onClick={() => onStatusChange(false)}
          >
            <Badge
              variant="outline"
              className="bg-red-100 text-red-700 border-red-200 mr-2"
            >
              TO DO
            </Badge>
            <span className="text-sm">Mark as TO DO</span>
          </Button>
          <Button
            variant="ghost"
            className="w-full justify-start"
            onClick={() => onStatusChange(true)}
          >
            <Badge
              variant="outline"
              className="bg-green-100 text-green-700 border-green-200 mr-2"
            >
              DONE
            </Badge>
            <span className="text-sm">Mark as DONE</span>
          </Button>
        </div>
      </PopoverContent>
    </Popover>
  );
}

