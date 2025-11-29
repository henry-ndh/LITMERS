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
import { Textarea } from '@/components/ui/textarea';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue
} from '@/components/ui/select';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { AlertTriangle } from 'lucide-react';
import { useCreateProject } from '@/queries/project.query';
import { useGetMyTeams } from '@/queries/team.query';
import { toast } from 'sonner';

interface CreateProjectDialogProps {
  teamId?: number;
  children?: ReactNode;
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
}

export default function CreateProjectDialog({
  teamId,
  children,
  open: controlledOpen,
  onOpenChange: controlledOnOpenChange
}: CreateProjectDialogProps) {
  const [internalOpen, setInternalOpen] = useState(false);
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [selectedTeamId, setSelectedTeamId] = useState<string>(teamId?.toString() || '');
  const [error, setError] = useState('');

  const isControlled = controlledOpen !== undefined;
  const open = isControlled ? controlledOpen : internalOpen;
  const setOpen = isControlled ? controlledOnOpenChange! : setInternalOpen;

  const { data: teamsData } = useGetMyTeams();
  const teams = teamsData?.data || teamsData || [];
  const createProjectMutation = useCreateProject();

  const handleCreate = async () => {
    if (!name.trim()) {
      setError('Please enter a project name');
      return;
    }

    if (!selectedTeamId) {
      setError('Please select a team');
      return;
    }

    setError('');

    try {
      await createProjectMutation.mutateAsync({
        teamId: parseInt(selectedTeamId),
        name: name.trim(),
        description: description.trim() || undefined
      });
      toast.success('Project created successfully');
      setName('');
      setDescription('');
      setSelectedTeamId(teamId?.toString() || '');
      setOpen(false);
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to create project');
      toast.error('Failed to create project');
    }
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      {children && <DialogTrigger asChild>{children}</DialogTrigger>}
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Create New Project</DialogTitle>
          <DialogDescription>
            Create a new project in your team to organize your work
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
            <Label htmlFor="team">Team *</Label>
            <Select
              value={selectedTeamId}
              onValueChange={setSelectedTeamId}
              disabled={!!teamId || createProjectMutation.isPending}
            >
              <SelectTrigger id="team">
                <SelectValue placeholder="Select a team" />
              </SelectTrigger>
              <SelectContent>
                {teams.map((team: any) => (
                  <SelectItem key={team.id} value={team.id.toString()}>
                    {team.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="name">Project Name *</Label>
            <Input
              id="name"
              placeholder="e.g., Marketing Campaign"
              value={name}
              onChange={(e) => setName(e.target.value)}
              disabled={createProjectMutation.isPending}
              maxLength={100}
              onKeyDown={(e) => {
                if (e.key === 'Enter') {
                  handleCreate();
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
              disabled={createProjectMutation.isPending}
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
              setOpen(false);
              setName('');
              setDescription('');
              setError('');
            }}
            disabled={createProjectMutation.isPending}
          >
            Cancel
          </Button>
          <Button onClick={handleCreate} disabled={createProjectMutation.isPending}>
            {createProjectMutation.isPending ? 'Creating...' : 'Create Project'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

