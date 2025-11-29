import { useState, useEffect } from 'react';
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
import { Textarea } from '@/components/ui/textarea';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { AlertTriangle } from 'lucide-react';
import { useUpdateProject } from '@/queries/project.query';
import { toast } from 'sonner';

interface Project {
  id: number;
  name: string;
  description?: string | null;
}

interface UpdateProjectDialogProps {
  project: Project | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export default function UpdateProjectDialog({
  project,
  open,
  onOpenChange
}: UpdateProjectDialogProps) {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [error, setError] = useState('');

  const updateProjectMutation = useUpdateProject();

  useEffect(() => {
    if (project) {
      setName(project.name || '');
      setDescription(project.description || '');
      setError('');
    }
  }, [project]);

  const handleUpdate = async () => {
    if (!project) return;

    if (!name.trim()) {
      setError('Please enter a project name');
      return;
    }

    setError('');

    try {
      await updateProjectMutation.mutateAsync({
        projectId: project.id,
        name: name.trim(),
        description: description.trim() || undefined
      });
      toast.success('Project updated successfully');
      onOpenChange(false);
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to update project');
      toast.error('Failed to update project');
    }
  };

  if (!project) return null;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Update Project</DialogTitle>
          <DialogDescription>
            Update project information
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
            <Label htmlFor="name">Project Name *</Label>
            <Input
              id="name"
              placeholder="e.g., Marketing Campaign"
              value={name}
              onChange={(e) => setName(e.target.value)}
              disabled={updateProjectMutation.isPending}
              maxLength={100}
              onKeyDown={(e) => {
                if (e.key === 'Enter') {
                  handleUpdate();
                }
              }}
            />
            <p className="text-xs text-muted-foreground">{name.length}/100 characters</p>
          </div>

          <div className="space-y-2">
            <Label htmlFor="description">Description</Label>
            <Textarea
              id="description"
              placeholder="Project description..."
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              disabled={updateProjectMutation.isPending}
              maxLength={2000}
              rows={4}
            />
            <p className="text-xs text-muted-foreground">
              {description.length}/2000 characters
            </p>
          </div>
        </div>

        <DialogFooter>
          <Button
            variant="outline"
            onClick={() => {
              onOpenChange(false);
              setError('');
            }}
            disabled={updateProjectMutation.isPending}
          >
            Cancel
          </Button>
          <Button onClick={handleUpdate} disabled={updateProjectMutation.isPending}>
            {updateProjectMutation.isPending ? 'Updating...' : 'Update Project'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

