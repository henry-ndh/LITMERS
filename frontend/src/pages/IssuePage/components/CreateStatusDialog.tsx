import { useState, ReactNode } from 'react';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { AlertTriangle } from 'lucide-react';
import { useCreateIssueStatus, useGetIssueStatusesByProjectId } from '@/queries/issue.query';
import { useToast } from '@/components/ui/use-toast';

interface CreateStatusDialogProps {
  projectId: number;
  children?: ReactNode;
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
}

const DEFAULT_COLORS = [
  '#3b82f6', // blue
  '#10b981', // green
  '#f59e0b', // amber
  '#ef4444', // red
  '#8b5cf6', // purple
  '#ec4899', // pink
  '#06b6d4', // cyan
  '#84cc16' // lime
];

export default function CreateStatusDialog({
  projectId,
  children,
  open: controlledOpen,
  onOpenChange: controlledOnOpenChange
}: CreateStatusDialogProps) {
  const { toast } = useToast();
  const [internalOpen, setInternalOpen] = useState(false);
  const [name, setName] = useState('');
  const [color, setColor] = useState(DEFAULT_COLORS[0]);
  const [wipLimit, setWipLimit] = useState<string>('');
  const [error, setError] = useState('');

  const isControlled = controlledOpen !== undefined;
  const open = isControlled ? controlledOpen : internalOpen;
  const setOpen = isControlled ? controlledOnOpenChange! : setInternalOpen;

  const { data: statusesData } = useGetIssueStatusesByProjectId(projectId);
  const statuses = statusesData?.data || statusesData || [];
  const createStatusMutation = useCreateIssueStatus();

  const handleCreate = async () => {
    if (!name.trim()) {
      setError('Please enter a status name');
      return;
    }

    setError('');

    try {
      await createStatusMutation.mutateAsync({
        projectId,
        name: name.trim(),
        color,
        position: statuses.length,
        isDefault: false,
        wipLimit: wipLimit ? parseInt(wipLimit) : undefined
      });
      toast({
        title: 'Success',
        description: 'Status created successfully'
      });
      setName('');
      setColor(DEFAULT_COLORS[0]);
      setWipLimit('');
      setOpen(false);
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to create status');
      toast({
        title: 'Error',
        description: 'Failed to create status',
        variant: 'destructive'
      });
    }
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      {children && <DialogTrigger asChild>{children}</DialogTrigger>}
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Create New Status</DialogTitle>
          <DialogDescription>
            Create a new status column for your Kanban board
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4 py-4">
          {error && (
            <Alert variant="destructive">
              <AlertTriangle className="h-4 w-4" />
              <AlertDescription>{error}</AlertDescription>
            </Alert>
          )}

          <div className="space-y-2">
            <Label htmlFor="name">Status Name *</Label>
            <Input
              id="name"
              placeholder="e.g., In Progress"
              value={name}
              onChange={(e) => setName(e.target.value)}
              disabled={createStatusMutation.isPending}
              maxLength={30}
            />
            <p className="text-xs text-muted-foreground">{name.length}/30 characters</p>
          </div>

          <div className="space-y-2">
            <Label htmlFor="color">Color</Label>
            <div className="flex gap-2">
              {DEFAULT_COLORS.map((c) => (
                <button
                  key={c}
                  type="button"
                  className={`h-10 w-10 rounded border-2 transition-all ${
                    color === c ? 'border-foreground scale-110' : 'border-gray-300'
                  }`}
                  style={{ backgroundColor: c }}
                  onClick={() => setColor(c)}
                  disabled={createStatusMutation.isPending}
                />
              ))}
            </div>
            <Input
              type="color"
              value={color}
              onChange={(e) => setColor(e.target.value)}
              disabled={createStatusMutation.isPending}
              className="w-full h-10"
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="wipLimit">WIP Limit (Optional)</Label>
            <Input
              id="wipLimit"
              type="number"
              placeholder="Leave empty for unlimited"
              value={wipLimit}
              onChange={(e) => setWipLimit(e.target.value)}
              disabled={createStatusMutation.isPending}
              min="1"
            />
            <p className="text-xs text-muted-foreground">
              Maximum number of issues allowed in this status
            </p>
          </div>
        </div>

        <DialogFooter>
          <Button
            variant="outline"
            onClick={() => {
              setOpen(false);
              setName('');
              setColor(DEFAULT_COLORS[0]);
              setWipLimit('');
              setError('');
            }}
            disabled={createStatusMutation.isPending}
          >
            Cancel
          </Button>
          <Button onClick={handleCreate} disabled={createStatusMutation.isPending}>
            {createStatusMutation.isPending ? 'Creating...' : 'Create Status'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

