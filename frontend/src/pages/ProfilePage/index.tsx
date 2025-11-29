import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle
} from '@/components/ui/card';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Separator } from '@/components/ui/separator';
import { useGetProfile, useUpdateProfile } from '@/queries/user.query';
import { useToast } from '@/components/ui/use-toast';
import { User, Mail, Save, ArrowLeft, X } from 'lucide-react';
import WorkspaceLayout from '@/components/layout/workspace-layout';

export default function ProfilePage() {
  const { toast } = useToast();
  const navigate = useNavigate();
  const [name, setName] = useState('');
  const [avatar, setAvatar] = useState('');
  const [isEditing, setIsEditing] = useState(false);

  const { data: profileData, isLoading } = useGetProfile();
  const updateProfileMutation = useUpdateProfile();

  const profile = profileData?.data || profileData;

  // Initialize form when profile data is loaded
  useEffect(() => {
    if (profile && !isEditing) {
      setName(profile.name || '');
      setAvatar(profile.avatar || '');
    }
  }, [profile, isEditing]);

  const getInitials = (name: string) => {
    if (!name) return 'U';
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  };

  const handleSave = async () => {
    if (!name.trim()) {
      toast({
        title: 'Error',
        description: 'Name is required',
        variant: 'destructive'
      });
      return;
    }

    if (name.length > 50) {
      toast({
        title: 'Error',
        description: 'Name must be less than 50 characters',
        variant: 'destructive'
      });
      return;
    }

    if (avatar && avatar.length > 500) {
      toast({
        title: 'Error',
        description: 'Avatar URL must be less than 500 characters',
        variant: 'destructive'
      });
      return;
    }

    try {
      await updateProfileMutation.mutateAsync({
        name: name.trim(),
        avatar: avatar || undefined
      });
      toast({
        title: 'Success',
        variant: 'success',
        description: 'Profile updated successfully'
      });
      setIsEditing(false);
    } catch (error: any) {
      toast({
        title: 'Error',
        description:
          error?.response?.data?.message ||
          error?.response?.data?.data ||
          'Failed to update profile',
        variant: 'destructive'
      });
    }
  };

  const handleCancel = () => {
    if (profile) {
      setName(profile.name || '');
      setAvatar(profile.avatar || '');
    }
    setIsEditing(false);
  };

  if (isLoading) {
    return (
      <WorkspaceLayout>
        <div className="flex flex-1 items-center justify-center">
          <div className="text-center">
            <p className="text-muted-foreground">Loading profile...</p>
          </div>
        </div>
      </WorkspaceLayout>
    );
  }

  return (
    <WorkspaceLayout>
      <div className="flex flex-1 flex-col overflow-auto bg-slate-50 p-6">
        <div className="mx-auto w-full max-w-3xl">
          {/* Header */}
          <div className="mb-6 flex items-center gap-4">
            <Button
              variant="ghost"
              size="sm"
              onClick={() => navigate(-1)}
              className="gap-2"
            >
              <ArrowLeft className="h-4 w-4" />
              Back
            </Button>
            <div>
              <h1 className="text-2xl font-bold">Profile</h1>
              <p className="text-sm text-muted-foreground">
                Manage your account information
              </p>
            </div>
          </div>

          <Card>
            <CardHeader>
              <CardTitle>Profile Information</CardTitle>
              <CardDescription>
                Update your profile information and avatar
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-6">
              {/* Avatar Section */}
              <div className="flex items-center gap-6">
                <Avatar className="h-24 w-24">
                  <AvatarImage src={avatar || profile?.avatar} />
                  <AvatarFallback className="text-lg">
                    {getInitials(name || profile?.name || '')}
                  </AvatarFallback>
                </Avatar>
                <div className="flex-1 space-y-2">
                  <Label>Avatar URL</Label>
                  <div className="flex items-center gap-2">
                    <Input
                      value={avatar || profile?.avatar || ''}
                      onChange={(e) => setAvatar(e.target.value)}
                      placeholder="https://example.com/avatar.jpg"
                      disabled={!isEditing}
                      className="flex-1"
                    />
                    {isEditing && (
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => setAvatar('')}
                      >
                        <X className="h-4 w-4" />
                      </Button>
                    )}
                  </div>
                  <p className="text-xs text-muted-foreground">
                    Enter a URL for your avatar image
                  </p>
                </div>
              </div>

              <Separator />

              {/* Name Section */}
              <div className="space-y-2">
                <Label htmlFor="name">Name</Label>
                <div className="relative">
                  <User className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                  <Input
                    id="name"
                    value={name || profile?.name || ''}
                    onChange={(e) => setName(e.target.value)}
                    placeholder="Enter your name"
                    disabled={!isEditing}
                    className="pl-9"
                    maxLength={50}
                  />
                </div>
                <p className="text-xs text-muted-foreground">
                  {name?.length || profile?.name?.length || 0}/50 characters
                </p>
              </div>

              {/* Email Section (Read-only) */}
              <div className="space-y-2">
                <Label htmlFor="email">Email</Label>
                <div className="relative">
                  <Mail className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                  <Input
                    id="email"
                    value={profile?.email || ''}
                    disabled
                    className="bg-muted pl-9"
                  />
                </div>
                <p className="text-xs text-muted-foreground">
                  Email cannot be changed
                </p>
              </div>

              {/* Action Buttons */}
              <div className="flex items-center justify-end gap-2 pt-4">
                {isEditing ? (
                  <>
                    <Button
                      variant="outline"
                      onClick={handleCancel}
                      disabled={updateProfileMutation.isPending}
                    >
                      Cancel
                    </Button>
                    <Button
                      onClick={handleSave}
                      disabled={updateProfileMutation.isPending}
                    >
                      <Save className="mr-2 h-4 w-4" />
                      {updateProfileMutation.isPending
                        ? 'Saving...'
                        : 'Save Changes'}
                    </Button>
                  </>
                ) : (
                  <Button onClick={() => setIsEditing(true)}>
                    Edit Profile
                  </Button>
                )}
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </WorkspaceLayout>
  );
}
