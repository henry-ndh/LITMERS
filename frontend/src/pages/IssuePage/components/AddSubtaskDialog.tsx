import { useState } from 'react';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { AlertTriangle } from 'lucide-react';
import { useCreateSubtask, useGetSubtasksByIssueId } from '@/queries/issue.query';
import { useToast } from '@/components/ui/use-toast';

interface AddSubtaskDialogProps {
  issueId: number;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export default function AddSubtaskDialog({
  issueId,
  open,
  onOpenChange
}: AddSubtaskDialogProps) {
  const { toast } = useToast();
  const [title, setTitle] = useState('');
  const [error, setError] = useState('');

  const { data: subtasksData } = useGetSubtasksByIssueId(issueId);
  const subtasks = subtasksData?.data || subtasksData || [];
  const createSubtaskMutation = useCreateSubtask();

  const handleCreate = async () => {
    if (!title.trim()) {
      setError('Please enter a subtask title');
      return;
    }

    setError('');

    try {
      await createSubtaskMutation.mutateAsync({
        issueId,
        title: title.trim(),
        position: subtasks.length
      });
      toast({
        title: 'Success',
        description: 'Subtask created successfully'
      });
      setTitle('');
      onOpenChange(false);
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to create subtask');
      toast({
        title: 'Error',
        description: 'Failed to create subtask',
        variant: 'destructive'
      });
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Add Subtask</DialogTitle>
          <DialogDescription>
            Add a new subtask to this issue
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
            <Label htmlFor="subtaskTitle">What needs to be done?</Label>
            <Input
              id="subtaskTitle"
              placeholder="Subtask title..."
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              disabled={createSubtaskMutation.isPending}
              maxLength={200}
              onKeyDown={(e) => {
                if (e.key === 'Enter') {
                  handleCreate();
                }
              }}
            />
            <p className="text-xs text-muted-foreground">{title.length}/200 characters</p>
          </div>
        </div>

        <DialogFooter>
          <Button
            variant="outline"
            onClick={() => {
              onOpenChange(false);
              setTitle('');
              setError('');
            }}
            disabled={createSubtaskMutation.isPending}
          >
            Cancel
          </Button>
          <Button onClick={handleCreate} disabled={createSubtaskMutation.isPending}>
            {createSubtaskMutation.isPending ? 'Creating...' : 'Create Subtask'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

