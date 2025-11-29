import { useEffect, useState } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle
} from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Alert, AlertDescription } from '@/components/ui/alert';
import {
  CheckCircle2,
  XCircle,
  Loader2,
  AlertTriangle,
  Users
} from 'lucide-react';
import { useAcceptInvite } from '@/queries/team.query';
import { toast } from '@/components/ui/use-toast';

export default function AcceptInvitePage() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const token = searchParams.get('token');
  const [isProcessing, setIsProcessing] = useState(false);

  const acceptInviteMutation = useAcceptInvite();
  const [isError, setIsError] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  useEffect(() => {
    if (!token) {
      return;
    }

    handleAcceptInvite();
  }, [token]);

  const handleAcceptInvite = async () => {
    if (!token) {
      return;
    }

    setIsProcessing(true);
    try {
      const [err] = await acceptInviteMutation.mutateAsync({ token });
      console.log(err);
      if (err) {
        toast({
          title: 'Error',
          description: err.data?.data || 'Failed to accept invitation',
          variant: 'destructive'
        });
        setIsError(true);
        setErrorMessage(err.data?.data);
      }
    } catch (error: any) {
    } finally {
      setIsProcessing(false);
    }
  };

  if (!token) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-background p-4">
        <Card className="w-full max-w-md">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <AlertTriangle className="h-5 w-5 text-destructive" />
              Error
            </CardTitle>
            <CardDescription>Invitation token not found</CardDescription>
          </CardHeader>
          <CardContent>
            <Alert variant="destructive">
              <AlertDescription>
                Invalid invitation link. Please check the link or request a new
                invitation.
              </AlertDescription>
            </Alert>
            <div className="mt-4">
              <Button
                variant="outline"
                onClick={() => navigate('/')}
                className="w-full"
              >
                Back to Team Page
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }
  if (acceptInviteMutation.isPending) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-background p-4">
        <Card className="w-full max-w-md">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Loader2 className="h-5 w-5 animate-spin" />
              Processing...
            </CardTitle>
          </CardHeader>
        </Card>
      </div>
    );
  }
  if (!isError) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-background p-4">
        <Card className="w-full max-w-md">
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-green-600">
              <CheckCircle2 className="h-5 w-5" />
              Success!
            </CardTitle>
            <CardDescription>
              You have successfully accepted the invitation
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <Alert className="border-green-200 bg-green-50">
              <CheckCircle2 className="h-4 w-4 text-green-600" />
              <AlertDescription className="text-green-800">
                You are now a member of the team.
              </AlertDescription>
            </Alert>
            <Button
              onClick={() => navigate('/')}
              className="w-full"
              variant="default"
            >
              Go to Team Page
            </Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  if (isError) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-background p-4">
        <Card className="w-full max-w-md">
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-destructive">
              <XCircle className="h-5 w-5" />
              Error
            </CardTitle>
            <CardDescription>Failed to accept invitation</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <Alert variant="destructive">
              <AlertTriangle className="h-4 w-4" />
              <AlertDescription>{errorMessage}</AlertDescription>
            </Alert>
            <div className="flex gap-2">
              <Button
                variant="outline"
                onClick={() => navigate('/')}
                className="flex-1"
              >
                Go Back
              </Button>
              <Button
                onClick={handleAcceptInvite}
                disabled={isProcessing}
                className="flex-1"
              >
                {isProcessing ? (
                  <>
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    Processing...
                  </>
                ) : (
                  'Try Again'
                )}
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-background p-4">
      <Card className="w-full max-w-md">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Users className="h-5 w-5" />
            Team Invitation
          </CardTitle>
          <CardDescription>
            You have received an invitation to join a team
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          {isProcessing ? (
            <div className="flex flex-col items-center justify-center py-8">
              <Loader2 className="mb-4 h-12 w-12 animate-spin text-primary" />
              <p className="text-muted-foreground">Processing invitation...</p>
            </div>
          ) : (
            <>
              <Alert>
                <Users className="h-4 w-4" />
                <AlertDescription>
                  Click the button below to accept the invitation and join the
                  team.
                </AlertDescription>
              </Alert>
              <Button
                onClick={handleAcceptInvite}
                disabled={isProcessing || acceptInviteMutation.isPending}
                className="w-full"
                size="lg"
              >
                {acceptInviteMutation.isPending ? (
                  <>
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    Processing...
                  </>
                ) : (
                  <>
                    <CheckCircle2 className="mr-2 h-4 w-4" />
                    Accept Invitation
                  </>
                )}
              </Button>
              <Button
                variant="outline"
                onClick={() => navigate('/')}
                className="w-full"
              >
                Cancel
              </Button>
            </>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
